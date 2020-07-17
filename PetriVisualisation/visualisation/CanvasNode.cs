using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using PetriVisualisation.Graph_Algorithms;
using SharpDX.DXGI;

namespace PetriVisualisation.visualisation
{
    public class CanvasNode<T> where T: Shape
    {
        public T shape;
        public TextBlock textBlock;
        public int width;
        public int height;
        public Tuple<int, int> middle;    //So i know width and height of node
        //! ports could be of type Avalonia.Point but for portability and later conversion to SVG Tuple does better
        public List<Tuple<int, int, bool>> portsLeft;    //Later for edges more complex edges
        public List<Tuple<int, int, bool>> portsRight;    //--||--
        //We usually enter the graph from top port and leave from the bottom port
        public Tuple<int, int> portOut;    
        public Tuple<int, int> portIn;
        public Node node;

        public CanvasNode(T ellipse, TextBlock textBlock, int width, int height, int x, int y, Node node)
        {
            shape = ellipse;
            this.textBlock = textBlock;
            this.width = width;
            this.height = height;
            middle = new Tuple<int, int>(x, y);
            this.node = node;
            portIn = new Tuple<int, int>(x, y-height/2);
            portOut = new Tuple<int, int>(x, y+height/2);
        }
    }
}