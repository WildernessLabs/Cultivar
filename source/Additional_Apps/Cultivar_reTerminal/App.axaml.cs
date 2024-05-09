using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Cultivar_reTerminal.ViewModels;
using Cultivar_reTerminal.Views;

namespace Cultivar_reTerminal
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                    //WindowState = Avalonia.Controls.WindowState.FullScreen
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}