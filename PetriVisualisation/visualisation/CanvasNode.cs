using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using PetriVisualisation.Graph_Algorithms;
using SharpDX.DXGI;

namespace PetriVisualisation.visualisation
{
    public class CanvasNode
    {
        public Shape shape;
        public NodeType type;
        public TextBlock textBlock;
        public int width;
        public int height;
        public System.Tuple<int, int> middle;    //So i know width and height of node
        //! ports could be of type Avalonia.Point
        private List<Point> _portsSide;    //Later for edges more complex edges
        //We usually enter the graph from top port and leave from the bottom port
        private Point _portOut;
        private Point _portIn;
        public readonly Node node;

        public CanvasNode(Shape shape, NodeType nodeType, TextBlock textBlock, int width, int height, int x, int y, Node node)
        {
            this.shape = shape;
            type = nodeType;
            this.textBlock = textBlock;
            this.width = width;
            this.height = height;
            middle = new System.Tuple<int, int>(x, y);
            this.node = node;
            CreatePorts(x, y);
        }

        // LT -> RT -> LB -> RB
        private void CreatePorts(int x, int y)
        {
            _portIn = new Point(x, y-height/2);
            _portOut = new Point(x, y+height/2);
            
            if (type == NodeType.Rectangle)
            {
                
                _portsSide = new List<Point>()
                {
                    new Point(middle.Item1 - width/2, middle.Item2 - height/2),
                    new Point(middle.Item1 + width/2, middle.Item2 - height/2),
                    new Point(middle.Item1 - width/2, middle.Item2 + height/2),
                    new Point(middle.Item1 + width/2, middle.Item2 + height/2)
                };
            }
            else if (type == NodeType.Ellipse)
            {
                _portsSide = new List<Point>()
                {
                    
                    new Point(middle.Item1 + (width/2)*Math.Cos(3.92699), middle.Item2 + (width/2)*Math.Sin(3.92699)),
                    new Point(middle.Item1 + (width/2)*Math.Cos(5.497787), middle.Item2 + (width/2)*Math.Sin(5.497787)),
                    new Point(middle.Item1 + (width/2)*Math.Cos(2.35619449), middle.Item2 + (width/2)*Math.Sin(2.35619449)),
                    new Point(middle.Item1 + (width/2)*Math.Cos(0.785398), middle.Item2 + (width/2)*Math.Sin(0.785398)),
                };
            }
            //TODO mora shapes later
        }

        public Point GetOutPort(CanvasNode getter)
        {
            if (getter.middle.Item2 > middle.Item2 && getter.middle.Item1 == middle.Item1)
                return _portOut;

            if (getter.middle.Item1 > middle.Item1)
                return getter.middle.Item2 > middle.Item2 ? _portsSide[3] : _portsSide[1];
            return getter.middle.Item2 > middle.Item2 ? _portsSide[2] : _portsSide[0];
        }
        
        public Point GetInPort(CanvasNode getter)
        {
            if (getter.middle.Item2 < middle.Item2 && getter.middle.Item1 == middle.Item1)
                return _portIn;
            return GetOutPort(getter);
        }
    }
}