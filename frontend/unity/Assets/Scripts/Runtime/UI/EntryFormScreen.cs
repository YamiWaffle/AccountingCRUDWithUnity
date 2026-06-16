using System;
using System.Threading;
using AccountingApp.Api;
using AccountingApp.Api.Models;
using AccountingApp.Store;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer.Unity;

namespace AccountingApp.UI
{
    public sealed class EntryFormScreen : IAsyncStartable, IDisposable
    {
        private readonly UIDocument _document;
        private readonly AccountingApiService _api;
        private readonly AccountingStore _store;

        private DisposableBag _disposables;

        private VisualElement _formPanel;
        private Label _formTitle;
        private TextField _amountField;
        private TextField _categoryField;
        private TextField _noteField;
        private TextField _dateField;
        private Label _errorLabel;

        private bool _isDisposed;

        public EntryFormScreen(UIDocument document, AccountingApiService api, AccountingStore store)
        {
            _document = document;
            _api = api;
            _store = store;
        }

        ~EntryFormScreen()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async UniTask StartAsync(CancellationToken ct)
        {
            await UniTask.NextFrame(ct);

            var root = _document.rootVisualElement;
            _formPanel = root.Q("form-panel");
            _formTitle = root.Q<Label>("form-title");
            _amountField = root.Q<TextField>("amount-field");
            _categoryField = root.Q<TextField>("category-field");
            _noteField = root.Q<TextField>("note-field");
            _dateField = root.Q<TextField>("date-field");
            _errorLabel = root.Q<Label>("form-error-label");

            var cancelBtn = root.Q<Button>("cancel-btn");
            var saveBtn = root.Q<Button>("save-btn");
            if (cancelBtn == null || saveBtn == null)
            {
                Debug.LogError("[EntryFormScreen] cancel-btn or save-btn not found in UXML");
                return;
            }

            cancelBtn.clicked += OnCancelButtonClick;
            saveBtn.clicked += OnSaveButtonClick;

            _store.IsFormVisible
                .Subscribe(OnVisibleFlagChanged)
                .AddTo(ref _disposables);
        }

        private void OnCancelButtonClick() => HideForm();

        private void OnSaveButtonClick() => SaveAsync(CancellationToken.None).Forget();

        private void OnVisibleFlagChanged(bool visible)
        {
            _formPanel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (visible)
                PopulateForm(_store.EditingEntry.Value);
        }

        private void PopulateForm(AccountEntryDto entry)
        {
            _errorLabel.style.display = DisplayStyle.None;

            if (entry == null)
            {
                _formTitle.text = "Add Entry";
                _amountField.value = "";
                _categoryField.value = "";
                _noteField.value = "";
                _dateField.value = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                _formTitle.text = "Edit Entry";
                _amountField.value = entry.Amount.ToString("F2");
                _categoryField.value = entry.Category;
                _noteField.value = entry.Note;
                _dateField.value = entry.Date?.Length >= 10 ? entry.Date[..10] : DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private async UniTaskVoid SaveAsync(CancellationToken ct)
        {
            _errorLabel.style.display = DisplayStyle.None;

            if (!double.TryParse(_amountField.value, out double amount) || amount <= 0)
            {
                ShowError("Invalid amount.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_categoryField.value))
            {
                ShowError("Category is required.");
                return;
            }

            if (!DateTime.TryParse(_dateField.value, out _))
            {
                ShowError("Invalid date format (YYYY-MM-DD).");
                return;
            }

            var dateStr = _dateField.value + "T00:00:00";

            try
            {
                var editing = _store.EditingEntry.Value;
                if (editing == null)
                {
                    await _api.CreateEntryAsync(new CreateEntryRequest
                    {
                        Amount = amount,
                        Category = _categoryField.value.Trim(),
                        Note = _noteField.value.Trim(),
                        Date = dateStr
                    });
                }
                else
                {
                    await _api.UpdateEntryAsync(new UpdateEntryRequest
                    {
                        Id = editing.Id,
                        Amount = amount,
                        Category = _categoryField.value.Trim(),
                        Note = _noteField.value.Trim(),
                        Date = dateStr
                    });
                }

                HideForm();
                _store.Entries.Value = await _api.GetEntriesAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[EntryFormScreen] Save failed: {e.Message}");
                ShowError("Save failed. Please check if the backend is running.");
            }
        }

        private void ShowError(string message)
        {
            _errorLabel.text = message;
            _errorLabel.style.display = DisplayStyle.Flex;
        }

        private void HideForm()
        {
            _store.EditingEntry.Value = null;
            _store.IsFormVisible.Value = false;
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                _disposables.Dispose();

            _isDisposed = true;
        }
    }
}
