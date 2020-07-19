using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using PetriVisualisation.Views;
using PetriVisualisation.visualisation;

namespace PetriVisualisation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string path = String.Empty;
        public async void BrowseFiles()
        {
            
            Path = await GetPath();
            if (!File.Exists(Path))
            {
                Path = string.Empty;
                return;
            }
            using var reader = new StreamReader(Path, Encoding.UTF8);
            Preview = await reader.ReadToEndAsync();
        }

        public async void Confirm()
        {
            if (Path == string.Empty)
                return;    //TODO make some nice "U have not choose any file" 
            var window = new Window();
            var worker = new MainCanvasWork();
            await worker.VisualiseGraph(window, Path);
            
            window.Show();
        }

        private async Task<string> GetPath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Text", Extensions =  { "dot" } });
        
            var result = await dialog.ShowAsync(new Window());
        
            return result != null ? string.Join(" ", result) : " ";
        }
    }
}
