using System;
using System.Collections.Generic;
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
    public sealed class EntryListScreen : IAsyncStartable, IDisposable
    {
        private readonly UIDocument _document;
        private readonly AccountingApiService _api;
        private readonly AccountingStore _store;

        private DisposableBag _disposables;

        private VisualElement _listPanel;
        private ListView _entriesList;
        private Label _emptyLabel;
        private Label _loadingLabel;

        private bool _isDisposed;

        public EntryListScreen(UIDocument document, AccountingApiService api, AccountingStore store)
        {
            _document = document;
            _api = api;
            _store = store;
        }

        ~EntryListScreen()
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
            _listPanel = root.Q("list-panel");
            _entriesList = root.Q<ListView>("entries-list");
            _emptyLabel = root.Q<Label>("empty-label");
            _loadingLabel = root.Q<Label>("loading-label");

            var addBtn = root.Q<Button>("add-btn");
            if (addBtn == null)
            {
                Debug.LogError("[EntryListScreen] 'add-btn' not found in UXML");
                return;
            }

            addBtn.clicked += OnAddButtonClicked;

            _entriesList.makeItem = MakeEntryRow;
            _entriesList.bindItem = BindEntryRow;
            _entriesList.fixedItemHeight = 68;
            _entriesList.selectionType = SelectionType.None;

            _store.IsFormVisible
                .Subscribe(OnVisibleFlagChanged)
                .AddTo(ref _disposables);

            _store.Entries
                .Subscribe(OnEntriesChanged)
                .AddTo(ref _disposables);

            _store.IsLoading
                .Subscribe(OnLoadingFlagChanged)
                .AddTo(ref _disposables);

            await RefreshEntriesAsync(ct);
        }

        private void OnAddButtonClicked()
        {
            _store.EditingEntry.Value = null;
            _store.IsFormVisible.Value = true;
        }

        private void OnVisibleFlagChanged(bool visible)
        {
            _listPanel.style.display = visible ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void OnEntriesChanged(List<AccountEntryDto> entries)
        {
            _entriesList.itemsSource = entries;
            _entriesList.Rebuild();
            _emptyLabel.style.display = entries.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _entriesList.style.display = entries.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnLoadingFlagChanged(bool loading)
        {
            _loadingLabel.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private async UniTask RefreshEntriesAsync(CancellationToken ct)
        {
            _store.IsLoading.Value = true;
            try
            {
                _store.Entries.Value = await _api.GetEntriesAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[EntryListScreen] Load failed: {e.Message}");
            }
            finally
            {
                _store.IsLoading.Value = false;
            }
        }

        private VisualElement MakeEntryRow()
        {
            var row = new VisualElement();
            row.AddToClassList("entry-row");
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.alignItems = Align.Center;

            // Left: 2-line info block
            var info = new VisualElement();
            info.style.flexGrow = 1f;
            info.style.flexShrink = 1f;
            info.style.overflow = Overflow.Hidden;

            // Line 1: category (left) + date (right)
            var line1 = new VisualElement();
            line1.style.flexDirection = FlexDirection.Row;
            line1.style.justifyContent = Justify.SpaceBetween;

            var categoryLabel = new Label { name = "category-label" };
            categoryLabel.AddToClassList("category");

            var dateLabel = new Label { name = "date-label" };
            dateLabel.AddToClassList("date");

            line1.Add(categoryLabel);
            line1.Add(dateLabel);

            // Line 2: amount (left) + note (right)
            var line2 = new VisualElement();
            line2.style.flexDirection = FlexDirection.Row;
            line2.style.justifyContent = Justify.SpaceBetween;
            line2.style.alignItems = Align.Center;

            var amountLabel = new Label { name = "amount-label" };
            amountLabel.AddToClassList("amount");

            var noteLabel = new Label { name = "note-label" };
            noteLabel.AddToClassList("note");

            line2.Add(amountLabel);
            line2.Add(noteLabel);

            info.Add(line1);
            info.Add(line2);

            // Right: buttons
            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.alignItems = Align.Center;
            buttons.style.flexShrink = 0f;
            buttons.style.marginLeft = 8f;

            var editBtn = new Button { text = "Edit" };
            editBtn.AddToClassList("btn-secondary");
            editBtn.clicked += () =>
            {
                if (row.userData is AccountEntryDto entry)
                {
                    _store.EditingEntry.Value = entry;
                    _store.IsFormVisible.Value = true;
                }
            };

            var deleteBtn = new Button { text = "Delete" };
            deleteBtn.AddToClassList("btn-danger");
            deleteBtn.clicked += () =>
            {
                if (row.userData is AccountEntryDto entry)
                    DeleteAsync(entry.Id).Forget();
            };

            buttons.Add(editBtn);
            buttons.Add(deleteBtn);

            row.Add(info);
            row.Add(buttons);
            return row;
        }

        private void BindEntryRow(VisualElement element, int index)
        {
            var entries = _store.Entries.Value;
            if (index >= entries.Count) return;

            var entry = entries[index];
            element.userData = entry;

            element.Q<Label>("amount-label").text = $"$ {entry.Amount:F2}";
            element.Q<Label>("category-label").text = entry.Category;
            element.Q<Label>("note-label").text = entry.Note;

            var datePart = entry.Date?.Length >= 10 ? entry.Date[..10] : entry.Date ?? "";
            element.Q<Label>("date-label").text = datePart;
        }

        private async UniTaskVoid DeleteAsync(int id)
        {
            try
            {
                await _api.DeleteEntryAsync(id);
                await RefreshEntriesAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError($"[EntryListScreen] Delete failed: {e.Message}");
            }
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
