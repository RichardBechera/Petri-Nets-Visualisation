using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PetriVisualisation.Graph_Algorithms;
using PetriVisualisation.ViewModels;
using PetriVisualisation.Views;

namespace PetriVisualisation
{
    public class App : Application
    {
        public override void Initialize()
        {
            //! added here so i could debug parser, proper avalonia using later
            Loader blah = new Loader();
            var graph = blah.LoadGraph("/home/richard/Downloads/petrinet18-02-2020_10-20-31.dot");
            var newGraph = Transform.transformGraph(graph);
            //! ends
            
            AvaloniaXamlLoader.Load(this);
        }

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
    }
}