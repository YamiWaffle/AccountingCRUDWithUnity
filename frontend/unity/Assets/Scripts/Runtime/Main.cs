using AccountingApp.Api;
using AccountingApp.Store;
using AccountingApp.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace AccountingApp
{
    public class Main : LifetimeScope
    {
        [SerializeField] 
        private UIDocument _uiDocument;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_uiDocument);
            builder.Register<AccountingApiService>(Lifetime.Singleton);
            builder.Register<AccountingStore>(Lifetime.Singleton);
            builder.RegisterEntryPoint<EntryListScreen>();
            builder.RegisterEntryPoint<EntryFormScreen>();
        }
    }
}
