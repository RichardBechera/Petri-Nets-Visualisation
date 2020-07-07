using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using PetriVisualisation.Views;

namespace PetriVisualisation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        

        public async void BrowseFiles()
        {
            string _path = await GetPath();
        }
        
        public async Task<string> GetPath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Text", Extensions =  { "dot" } });

            string[] result = await dialog.ShowAsync(new Window());

            return string.Join(" ", result);
        }
    }
}
