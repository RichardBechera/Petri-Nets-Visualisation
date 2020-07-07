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
        private string path = String.Empty;
        public async void BrowseFiles()
        {
            
            Path = await GetPath();
            using (var reader = System.IO.File.OpenText(Path))
            {
                 Preview = await reader.ReadToEndAsync();            
            }
        }

        public async void Confirm()
        {
            
            //Preview = await System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");
            // var choose = new Window();
            // var text = new TextBox();
            // text.Text = path;
            // choose.Content = text;
            // choose.Show();

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
