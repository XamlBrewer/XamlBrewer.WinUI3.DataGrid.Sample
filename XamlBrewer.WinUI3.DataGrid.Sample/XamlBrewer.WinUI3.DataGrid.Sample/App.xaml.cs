using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using XamlBrewer.WinUI3.Services;

namespace XamlBrewer.WinUI3.DataGrid.Sample
{
    public partial class App : Application
    {
        private Shell shell;

        public static string Title => "XAML Brewer WinUI 3 DataGrid Sample";

        public App()
        {
            InitializeComponent();
        }

        public INavigation Navigation => shell;

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Ioc.Default.ConfigureServices
                (new ServiceCollection()
                    .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                    .BuildServiceProvider()
                );

            shell = new Shell();
            shell.Activate();
        }
    }
}
