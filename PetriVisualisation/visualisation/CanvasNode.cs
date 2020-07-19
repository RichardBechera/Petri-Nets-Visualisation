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
        public Tuple<int, int> middle;    //So i know width and height of node
        //! ports could be of type Avalonia.Point
        private List<Point> portsSide;    //Later for edges more complex edges
        //We usually enter the graph from top port and leave from the bottom port
        public Point portOut;    
        public Point portIn;
        public Node node;

        public CanvasNode(Shape shape, NodeType nodeType, TextBlock textBlock, int width, int height, int x, int y, Node node)
        {
            this.shape = shape;
            type = nodeType;
            this.textBlock = textBlock;
            this.width = width;
            this.height = height;
            middle = new Tuple<int, int>(x, y);
            this.node = node;
            createPorts(x, y);
        }

        // LT -> RT -> LB -> RB
        private void createPorts(int x, int y)
        {
            portIn = new Point(x, y-height/2);
            portOut = new Point(x, y+height/2);
            
            if (type == NodeType.Rectangle)
            {
                
                portsSide = new List<Point>()
                {
                    new Point(middle.Item1 - width/2, middle.Item2 - height/2),
                    new Point(middle.Item1 + width/2, middle.Item2 - height/2),
                    new Point(middle.Item1 - width/2, middle.Item2 + height/2),
                    new Point(middle.Item1 + width/2, middle.Item2 + height/2)
                };
            }
            else if (type == NodeType.Ellipse)
            {
                portsSide = new List<Point>()
                {
                    
                    new Point(middle.Item1 + (width/2)*Math.Cos(3.92699), middle.Item2 + (width/2)*Math.Sin(3.92699)),
                    new Point(middle.Item1 + (width/2)*Math.Cos(5.497787), middle.Item2 + (width/2)*Math.Sin(5.497787)),
                    new Point(middle.Item1 + (width/2)*Math.Cos(2.35619449), middle.Item2 + (width/2)*Math.Sin(2.35619449)),
                    new Point(middle.Item1 + (width/2)*Math.Cos(0.785398), middle.Item2 + (width/2)*Math.Sin(0.785398)),
                };
            }
            //TODO mora shapes later
        }

        public Point getOutPort(CanvasNode getter)
        {
            if (getter.middle.Item2 > middle.Item2 && getter.middle.Item1 == middle.Item1)
                return portOut;

            if (getter.middle.Item1 > middle.Item1)
                return getter.middle.Item2 > middle.Item2 ? portsSide[3] : portsSide[1];
            return getter.middle.Item2 > middle.Item2 ? portsSide[2] : portsSide[0];
        }
        
        public Point getInPort(CanvasNode getter)
        {
            if (getter.middle.Item2 < middle.Item2 && getter.middle.Item1 == middle.Item1)
                return portIn;
            return getOutPort(getter);
        }
    }
}