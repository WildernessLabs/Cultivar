using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Meadow;
using Meadow.UI;
using ProjectLabSimulator.ViewModels;
using ProjectLabSimulator.Views;
using System.Threading.Tasks;

namespace ProjectLabSimulator
{
    public partial class App : AvaloniaMeadowApplication<Windows>
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            LoadMeadowOS();
        }

        public override Task MeadowInitialize()
        {
            return Task.CompletedTask;
        }
    }
}