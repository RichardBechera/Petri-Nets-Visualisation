using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PetriVisualisation.Graph_Algorithms;
using Transform = PetriVisualisation.Graph_Algorithms.Transform;

namespace PetriVisualisation.visualisation
{
    public class MainCanvasWork
    {
        private Graph_Algorithms.Graph _graph;

        public void VisualiseGraph(Window window, string path)
        {
            Loader loader = new Loader();
            var graph = loader.LoadGraph(path);
            _graph = Transform.transformGraph(graph);
            var topo = algos.getTopoOnScc(graph, _graph);
            var canvas = new Canvas();
            canvas.Background = Brushes.White;
            canvas.Width = algos.widthOfNet(topo) * 85 + 150;
            canvas.Height = algos.heightOfNet(topo) * 70 + 35;    //TODO this seem like a little bit too much
            var shapes = TraverseComponents(topo, 35, (int)canvas.Width / 2);
            var arrows = AddEdges(shapes.nodes, shapes.edges);
            //!disgusting
            foreach (var node in shapes.nodes)
            {
                canvas.Children.Add(node.shape);
                canvas.Children.Add(node.textBlock);
            }
            foreach (var edge in shapes.edges)
            {
                canvas.Children.Add(edge.shape);
                canvas.Children.Add(edge.textBlock);
            }
            foreach (var arrow in arrows)
            {
                canvas.Children.Add(arrow.Item1);
                canvas.Children.Add(arrow.Item2);
            }

            window.Background = Brushes.Black;
            window.Opacity = 0.8;
            window.Content = canvas;
        }

        private List<(Line, Polygon)> AddEdges(List<CanvasNode<Ellipse>> nodes, List<CanvasNode<Rectangle>> edges)
        {
            var arrows = (from node in nodes 
                from succ 
                    in node.node.succs 
                select CreateLine(edges
                    .Find(e => e.node.attr.id == succ.Item1.attr.id)?.portIn, node.portOut))
                .ToList();
            arrows.AddRange(from edge in edges 
                from succ 
                    in edge.node.succs 
                select CreateLine(nodes
                    .Find(e => e.node.attr.id == succ.Item1.attr.id)?.portIn, edge.portOut));

            return arrows;
        }

        private (Line line, Polygon tip) CreateLine(Tuple<int, int> head, Tuple<int, int> tail)
        {
            var line = new Line
            {
                StartPoint = new Point(tail.Item1, tail.Item2),
                EndPoint = new Point(head.Item1, head.Item2),
                Stroke = Brushes.Black
            };

            //make arrows less retarded
            var unitVector = (tail.Item1 - head.Item1, tail.Item2 - head.Item2);
            var unitVectorMagnitude = Math.Floor(Math.Sqrt(Math.Pow(unitVector.Item1, 2) + Math.Pow(unitVector.Item2, 2)));
            unitVector = ((int)Math.Floor(unitVector.Item1/unitVectorMagnitude) * 5, (int)Math.Floor(unitVector.Item2/unitVectorMagnitude) * 5);
            var middlePoint = (head.Item1 + unitVector.Item1, head.Item2 + unitVector.Item2);
            var leftPoint = new Point(middlePoint.Item1 + unitVector.Item2, middlePoint.Item2 - unitVector.Item1 );
            var rightPoint = new Point(middlePoint.Item1 - unitVector.Item2, middlePoint.Item2 + unitVector.Item1 );

            var tip = new Polygon
            {
                Points = new List<Point> {line.EndPoint, leftPoint, rightPoint}, Fill = Brushes.Black
            };
            return (line, tip);
        }

        private (List<CanvasNode<Ellipse>> nodes, List<CanvasNode<Rectangle>> edges) 
            TraverseComponents(List<StrongComponent> components, int top, int left)
        {
            if (components.Count < 1)
                return (null, null);
            var edges = new List<CanvasNode<Rectangle>>();
            var nodes = new List<CanvasNode<Ellipse>>();
            components = algos.sortComponentTopology(components);
            var bag = new List<StrongComponent>();
            var current = 0;
            foreach (var c in components)
            {
                if (current != c.depth)
                {
                    TraverseBag(bag, edges, nodes, ref top, ref left);
                    current = c.depth;
                    bag.Clear();
                }
                bag.Add(c);
            }
            TraverseBag(bag, edges, nodes, ref top, ref left);
            return (nodes, edges);
        }

        private void TraverseBag(List<StrongComponent> bag, List<CanvasNode<Rectangle>> edges,
            List<CanvasNode<Ellipse>> nodes, ref int top, ref int left)
        {
            var leftOffset = (left * 2) / (bag.Count+1);
            var leftPosition = leftOffset;
            var height = 1;
            //var topOffset
            //TODO for more thn 1 node in strong component
            foreach (var node in bag)//(var node in from sc in bag where sc.nodes.Count == 1 select sc.nodes.First()) from time whn i assumed scc to have 1 node
            {
                
                var fromSub = string.Empty;
                _graph.subgraphs.Find(sub => sub.id == node.attr.belonging)?
                    .NodeAttr.TryGetValue("shape", out fromSub);    //TODO apply all attributes ? (graph => (sub)^+graph => node
                switch (fromSub)
                {
                    case "rect":
                        edges.Add(createRectangle(node.attr.id, top, leftPosition, node));
                        break;
                    case "circle":
                        nodes.Add(createEllipse(node.attr.id, top, leftPosition, node));
                        break;
                    //TODO Any other shapes ?
                }

                leftPosition += leftOffset;
            }
            //moved top down, as low as heighest component
            top += bag.Aggregate(1, (i, component) => i < component.nodes.Count ? component.nodes.Count : i) * 60;
        }

        private CanvasNode<Ellipse> createEllipse(string name, int top, int left, Node node)
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
            return new CanvasNode<Ellipse>(circle, text, 50, 50, left, top, node);
        }

        private CanvasNode<Rectangle> createRectangle(string name, int top, int left, Node node)
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
            return new CanvasNode<Rectangle>(rectangle, text, 50, 30, left, top, node);
        }
    }
}