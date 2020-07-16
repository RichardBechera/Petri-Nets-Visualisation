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
            using var reader = new StreamReader(Path, Encoding.UTF8);
            Preview = await reader.ReadToEndAsync();
        }

        public void Confirm()
        {
            var window = new Window();
            var worker = new MainCanvasWork();
            worker.VisualiseGraph(window, Path);
             
             // var node = createEllipse("p0", 200, 200);
             // var edge = createRectangle("a", 270, 200);
             // var line = new Line();
             // line.StartPoint = new Point(200, 225);
             // line.EndPoint = new Point(200, 258);
             // // Canvas.SetTop(line, 225);
             // // Canvas.SetLeft(line, 200);
             // line.Stroke = Brushes.Black;
             // // line.StrokeThickness = 3;
             // // line.Fill= Brushes.Black;
             // myCanvas.Children.Add(node.Item1);
             // myCanvas.Children.Add(node.Item2);
             // myCanvas.Children.Add(edge.Item1);
             // myCanvas.Children.Add(edge.Item2);
             // myCanvas.Children.Add(line);
             // choose.Content = myCanvas;
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
