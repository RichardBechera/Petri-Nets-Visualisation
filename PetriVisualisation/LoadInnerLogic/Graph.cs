using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PetriVisualisation.Graph_Algorithms;

namespace PetriVisualisation
{
    
    //TODO Type enums merge into less enums, move into separate folders/files
    public enum GraphType
    {
        Digraph,
        Graph,
        Empty
    }

    public enum Enter
    {
        Enter,
        Leave
    }

    public enum Rule
    {
        Graph,
        Node,
        Subgraph, 
        Id,
        Port,    //unused
        AttrList,
        AttrStmt,
        Alist,
        EdgeRhs,
        EdgeStmt
        
    }

    public enum AttrType
    {
        Graph,
        Node,
        Edge,
        EdgeStmt,
        None
    }

    public abstract class IGraph
    {
        public List<IGraph> succs = new List<IGraph>();
        public Dictionary<string, string> GraphAttr = new Dictionary<string, string>();
        public Dictionary<string, string> NodeAttr = new Dictionary<string, string>();
        public Dictionary<string, string> EdgeAttr = new Dictionary<string, string>();
        public string id = "";
    }

    public class Subgraph : IGraph
    {
        //private string _id;
        public string belonging;
    }

    public class DotNode : IGraph
    {
        private int port;
        public string belonging;
    }
    public class Graph : IGraph
    {
        public bool _strict = false;
        
        public List<IGraph> subgraphs = new List<IGraph>();
        
        public List<Edge> edges = new List<Edge>();
        public GraphType _type { get; set; }

        //not needed, succs only contain nodes
        public List<DotNode> onlyNodes() => succs
            .Where(graph => new Typecheck<IGraph, DotNode>(graph).CanConvert)
                .ToList()
                .ConvertAll(node => (DotNode) node);
        
        //not needed, subgraphs only contain subgraphs
        public List<Subgraph> onlySubgraphs() => subgraphs
            .Where(graph => new Typecheck<IGraph, Subgraph>(graph).CanConvert)
            .ToList()
            .ConvertAll(node => (Subgraph) node);
    }

    public class Edge
    {
        public string headId;
        public string tailId;
        public Dictionary<string, string> EdgeAttr = new Dictionary<string, string>();
    }




    /*
graph	    :	[ strict ] (graph | digraph) [ ID ] '{' stmt_list '}'
stmt_list	:	[ stmt [ ';' ] stmt_list ]
stmt	    :	node_stmt
             |	edge_stmt
             |	attr_stmt
             |	ID '=' ID
             |	subgraph
attr_stmt	:	(graph | node | edge) attr_list
attr_list	:	'[' [ a_list ] ']' [ attr_list ]
a_list	    :	ID '=' ID [ (';' | ',') ] [ a_list ]
edge_stmt	:	(node_id | subgraph) edgeRHS [ attr_list ]
edgeRHS	    :	edgeop (node_id | subgraph) [ edgeRHS ]
node_stmt	:	node_id [ attr_list ]
node_id	    :	ID [ port ]
port	    :	':' ID [ ':' compass_pt ]
             |	':' compass_pt
subgraph	:	[ subgraph [ ID ] ] '{' stmt_list '}'
compass_pt	:	(n | ne | e | se | s | sw | w | nw | c | _)
     */
}