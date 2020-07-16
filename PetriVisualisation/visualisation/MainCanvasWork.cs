using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PetriVisualisation.Graph_Algorithms;

namespace PetriVisualisation.visualisation
{
    public class MainCanvasWork
    {

        public void VisualiseGraph(Window window, string path)
        {
            Loader loader = new Loader();
            var graph = loader.LoadGraph(path);
            var topo = algos.getTopoOnScc(graph);
            var canvas = new Canvas();
            canvas.Background = Brushes.White;
            canvas.Width = algos.widthOfNet(topo) * 85 + 200;
            canvas.Height = algos.heightOfNet(topo) * 150 + 200;    //TODO this seem like a little bit too much
            
            window.Background = Brushes.Black;
            window.Opacity = 0.8;
            window.Content = canvas;
        }

        private void traverseComponents(List<StrongComponent> components)
        {
            return;
            ;
        }

        private (TextBlock, Ellipse) createEllipse(string name, int top, int left)
        {
            var circle =  new Ellipse()
            {
                Fill = Brushes.Aquamarine,
                Height = 50,
                Width = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Name = name
            };
            var text = new TextBlock()
            {
                Text = name,
                Width = 40,
                Height = 20,
                FontWeight = FontWeight.Black,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetTop(circle, top-25);
            Canvas.SetLeft(circle, left-25);
            Canvas.SetTop(text, top-10);
            Canvas.SetLeft(text, left-20);
            text.ZIndex = circle.ZIndex + 1;
            return (text, circle);
        }

        private (TextBlock, Rectangle) createRectangle(string name, int top, int left)
        {
            var rectangle =  new Rectangle()
            {
                Fill = Brushes.Cyan,
                Height = 30,
                Width = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Name = name
            };
            var text = new TextBlock()
            {
                Text = name,
                Width = 44,
                Height = 20,
                FontWeight = FontWeight.Black,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetTop(text, top-10);
            Canvas.SetLeft(text, left-22);
            Canvas.SetTop(rectangle, top-15);
            Canvas.SetLeft(rectangle, left-25);
            text.ZIndex = rectangle.ZIndex + 1;
            return (text, rectangle);
        }
    }
}