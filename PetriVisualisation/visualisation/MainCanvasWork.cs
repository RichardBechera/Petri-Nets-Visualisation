using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private List<(Line, Polygon)> AddEdges(List<CanvasNode> nodes, List<CanvasNode> edges)
        {
            var arrows = (from node in nodes 
                from succ 
                    in node.node.succs 
                select CreateLine(edges
                    .Find(e => e.node.attr.id == succ.Item1.attr.id), node))
                .ToList();
            arrows.AddRange(from edge in edges 
                from succ 
                    in edge.node.succs 
                select CreateLine(nodes
                    .Find(e => e.node.attr.id == succ.Item1.attr.id), edge));

            return arrows;
        }

        private (Line line, Polygon tip) CreateLine(CanvasNode toNode, CanvasNode fromNode)
        {
            var head = toNode.getInPort(fromNode);
            var tail = fromNode.getOutPort(toNode);
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

        private (List<CanvasNode> nodes, List<CanvasNode> edges) 
            TraverseComponents(List<StrongComponent> components, int top, int left)
        {
            if (components.Count < 1)
                return (null, null);
            var edges = new List<CanvasNode>();
            var nodes = new List<CanvasNode>();
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

        private void TraverseBag(List<StrongComponent> bag, List<CanvasNode> edges,
            List<CanvasNode> nodes, ref int top, ref int left)
        {
            var leftOffset = (left * 2) / (bag.Count+1);
            var leftPosition = leftOffset;
            var height = 0;
            //var topOffset
            //TODO for more thn 1 node in strong component
            foreach (var node in bag)//(var node in from sc in bag where sc.nodes.Count == 1 select sc.nodes.First()) from time whn i assumed scc to have 1 node
            {
                var orderedC = algos.SccOrdering(node);
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
                            edges.Add(createRectangle(nodeW.attr.id, top, leftPosition, nodeW));
                            break;
                        case "circle":
                            nodes.Add(createEllipse(nodeW.attr.id, top, leftPosition, nodeW));
                            break;
                        //TODO Any other shapes ?
                    }
                }
                else
                {
                    VisualiseBigComponent(orderedC, edges, nodes, top, leftPosition, leftOffset);
                }
                leftPosition += leftOffset;
            }
            //moved top down, as low as heighest component
            top += (height + 1) * 60;
        }

        private void VisualiseBigComponent(List<Tuple<Node, int, int>> order, List<CanvasNode> edges,
            List<CanvasNode> nodes, int top, int middle, int widthOfA)
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
                        edges.Add(createRectangle(node.attr.id, topPosition, leftPosition, node));
                        break;
                    case "circle":
                        nodes.Add(createEllipse(node.attr.id, topPosition, leftPosition, node));
                        break;
                    //TODO Any other shapes ?
                }
            }
        } 

        private CanvasNode createEllipse(string name, int top, int left, Node node)
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
            return new CanvasNode(circle, NodeType.Ellipse, text, 50, 50, left, top, node);
        }

        private CanvasNode createRectangle(string name, int top, int left, Node node)
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
            return new CanvasNode(rectangle, NodeType.Rectangle, text, 50, 30, left, top, node);
        }
    }
}