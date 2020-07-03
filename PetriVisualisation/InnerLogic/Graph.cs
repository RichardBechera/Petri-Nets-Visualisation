using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private string _id;
    }

    public class Node : IGraph
    {
        private int port;
    }
    public class Graph : IGraph
    {
        public bool _strict = false;
        public GraphType _type { get; set; }
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