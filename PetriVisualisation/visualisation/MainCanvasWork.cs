using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PetriVisualisation.Graph_Algorithms;
using PetriVisualisation.LoadInnerLogic;
using Transform = PetriVisualisation.Graph_Algorithms.Transform;

namespace PetriVisualisation.visualisation
{
    public class MainCanvasWork
    {
        private Graph_Algorithms.Graph _graph;
        public async Task VisualiseGraph(Window window, string path)
        {
            var loader = new Loader();
            var graph = await loader.LoadGraphAsync(path);
            _graph = Transform.transformGraph(graph);
            var topo = algos.getTopoOnScc(graph, _graph);
            var canvas = new Canvas
            {
                Background = Brushes.White,
                Width = algos.WidthOfNet(topo) * 85 + 150,
                Height = algos.HeightOfNet(topo) * 70 + 35
            };
            //TODO this seem like a little bit too much
            var shapes = TraverseComponents(topo, 35, (int)canvas.Width / 2);
            var arrows = AddEdges(shapes);
            foreach (var node in shapes)
            {
                canvas.Children.Add(node.shape);
                canvas.Children.Add(node.textBlock);
            }
            foreach (var (line, tip) in arrows)
            {
                canvas.Children.Add(line);
                canvas.Children.Add(tip);
            }
            
            var scroller = new ScrollViewer {Content = canvas};
            scroller.Width = 400;
            scroller.Height = 600;

            window.Background = Brushes.Black;
            window.Opacity = 0.8;
            window.Content = scroller;
        }

        private List<(Line, Polygon)> AddEdges(List<CanvasNode> nodes)
        {
            var arrows = nodes
                .SelectMany(a => a.node.succs
                    .Select(succ => nodes
                        .Find(can => succ.Item1.attr.id == can.node.attr.id))
                    .Select(head => CreateLine(head, a)))
                .ToList();

            return arrows;
        }

        private (Line line, Polygon tip) CreateLine(CanvasNode toNode, CanvasNode fromNode)
        {
            var head = toNode.GetInPort(fromNode);
            var tail = fromNode.GetOutPort(toNode);
            var line = new Line
            {
                StartPoint = tail,
                EndPoint = head,
                Stroke = Brushes.Black
            };

            //make arrows less retarded
            var unitVector = (tail.X - head.X, tail.Y - head.Y);
            var unitVectorMagnitude = Math.Floor(Math.Sqrt(Math.Pow(unitVector.Item1, 2) + Math.Pow(unitVector.Item2, 2)));
            unitVector = ((int)Math.Floor(unitVector.Item1/unitVectorMagnitude) * 5, (int)Math.Floor(unitVector.Item2/unitVectorMagnitude) * 5);
            var middlePoint = (head.X + unitVector.Item1, head.Y + unitVector.Item2);
            var leftPoint = new Point(middlePoint.Item1 + unitVector.Item2, middlePoint.Item2 - unitVector.Item1 );
            var rightPoint = new Point(middlePoint.Item1 - unitVector.Item2, middlePoint.Item2 + unitVector.Item1 );

            var tip = new Polygon
            {
                Points = new List<Point> {line.EndPoint, leftPoint, rightPoint}, Fill = Brushes.Black
            };
            return (line, tip);
        }

        private List<CanvasNode> TraverseComponents(List<StrongComponent> components, int top, int left)
        {
            if (components.Count < 1)
                return null;
            var nodes = new List<CanvasNode>();
            components = algos.SortComponentTopology(components);
            var bag = new List<StrongComponent>();
            var current = 0;
            foreach (var c in components)
            {
                if (current != c.depth)
                {
                    TraverseBag(bag, nodes, ref top, ref left);
                    current = c.depth;
                    bag.Clear();
                }
                bag.Add(c);
            }
            TraverseBag(bag, nodes, ref top, ref left);
            return nodes;
        }

        private void TraverseBag(List<StrongComponent> bag, List<CanvasNode> nodes, ref int top, ref int left)
        {
            var leftOffset = (left * 2) / (bag.Count+1);
            var leftPosition = leftOffset;
            var height = 0;
            foreach (var orderedC in bag.Select(node => algos.SccOrdering(node)))
            {
                if (orderedC.Last().Item2 > height)
                    height = orderedC.Last().Item2;
                if (orderedC.Count == 1)
                {
                    var nodeW = orderedC.First().Item1;
                    var fromSub = string.Empty;
                    _graph.subgraphs.Find(sub => sub.id == nodeW.attr.belonging)?
                        .NodeAttr
                        .TryGetValue("shape", out fromSub); //TODO apply all attributes ? (graph => (sub)^+graph => node
                    switch (fromSub)
                    {
                        case "rect":
                            nodes.Add(createRectangle(top, leftPosition, nodeW));    
                            break;
                        case "circle":
                            nodes.Add(createEllipse(top, leftPosition, nodeW));
                            break;
                        //TODO Any other shapes ?
                    }
                }
                else
                {
                    VisualiseBigComponent(orderedC, nodes, top, leftPosition, leftOffset);
                }
                leftPosition += leftOffset;
            }
            //moved top down, as low as heighest component
            top += (height + 1) * 60;
        }

        private void VisualiseBigComponent(List<Tuple<Node, int, int>> order, List<CanvasNode> nodes, int top, int middle, int widthOfA)
        {
            var left = middle - widthOfA / 4;
            var right = middle + widthOfA / 4;
            foreach (var (node, ord, s) in order)
            {
                var leftPosition = s switch
                {
                    0 => left,
                    1 => middle,
                    _ => right
                };
                var topPosition = top + ord * 60; 
                var fromSub = string.Empty;
                _graph.subgraphs.Find(sub => sub.id == node.attr.belonging)?
                    .NodeAttr
                    .TryGetValue("shape", out fromSub); //TODO apply all attributes ? (graph => (sub)^+graph => node; as above
                switch (fromSub)
                {
                    case "rect":
                        nodes.Add(createRectangle(topPosition, leftPosition, node));    
                        break;
                    case "circle":
                        nodes.Add(createEllipse(topPosition, leftPosition, node));
                        break;
                    //TODO Any other shapes ?
                }
            }
        } 

        private CanvasNode createEllipse(int top, int left, Node node)
        {
            var circle =  new Ellipse()
            {
                Fill = Brushes.Aquamarine,
                Height = 50,
                Width = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Name = node.attr.id
            };
            var text = new TextBlock()
            {
                Text = node.attr.id,
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
            return new CanvasNode(circle, NodeType.Ellipse, text, 50, 50, left, top, node);
        }

        private CanvasNode createRectangle(int top, int left, Node node)
        {
            var rectangle =  new Rectangle()
            {
                Fill = Brushes.Cyan,
                Height = 30,
                Width = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Name = node.attr.id
            };
            var text = new TextBlock()
            {
                Text = node.attr.id,
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
            return new CanvasNode(rectangle, NodeType.Rectangle, text, 50, 30, left, top, node);
        }
    }
}