using System;
using System.Collections.Generic;
using PetriVisualisation.LoadInnerLogic;

namespace PetriVisualisation.Graph_Algorithms
{
    public class Graph
    {
        public bool _strict = false;
        public List<Node> nodes = new List<Node>();
        public List<Attributes> subgraphs = new List<Attributes>();
        public Attributes attr = new Attributes();
        //possibly List<Edge> could help, now not needed
        public GraphType _type { get; set; }

        public Graph(GraphType type, Attributes attr, bool strict)
        {
            _type = type;
            _strict = strict;
            this.attr = attr;
        }
        
        public Graph() {
        }
    }

    public class Attributes
    {
        public string id = "";
        public string belonging = "";
        public Dictionary<string, string> GraphAttr = new Dictionary<string, string>();
        public Dictionary<string, string> NodeAttr = new Dictionary<string, string>();
        public Dictionary<string, string> EdgeAttr = new Dictionary<string, string>();

        public Attributes(string id, string belonging)
        {
            this.id = id;
            this.belonging = belonging;
        }

        public Attributes(string id, string belonging, Dictionary<string, string> graphAttr, Dictionary<string, string> nodeAttr, Dictionary<string,string> edgeAttr)
        {
            this.belonging = belonging;
            this.id = id;
            EdgeAttr = edgeAttr;
            GraphAttr = graphAttr;
            NodeAttr = nodeAttr;
        }

        public Attributes()
        {
        }
    }

    public class Node
    {
        public Attributes attr = new Attributes();
        public string port; //unused
        public List<System.Tuple<Node, Dictionary<string, string>>> succs = new List<System.Tuple<Node, Dictionary<string, string>>>();
    }
    
}