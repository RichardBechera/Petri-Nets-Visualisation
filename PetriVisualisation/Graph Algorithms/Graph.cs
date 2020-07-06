using System;
using System.Collections.Generic;

namespace PetriVisualisation.Graph_Algorithms
{
    public class Graph
    {
        public bool _strict = false;
        public List<Node> nodes = new List<Node>();
        public List<Attributes> subgraphs = new List<Attributes>();
        public GraphType _type { get; set; }
    }

    public class Attributes
    {
        public string id = "";
        public string belonging = "";
        public Dictionary<string, string> GraphAttr = new Dictionary<string, string>();
        public Dictionary<string, string> NodeAttr = new Dictionary<string, string>();
        public Dictionary<string, string> EdgeAttr = new Dictionary<string, string>();
    }

    public class Node
    {
        public Attributes attr = new Attributes();
        public List<Tuple<Node, Dictionary<string, string>>> succs = new List<Tuple<Node, Dictionary<string, string>>>();
    }
    
}