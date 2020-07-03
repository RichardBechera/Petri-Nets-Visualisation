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

    public enum AttrType
    {
        Graph,
        Node,
        Edge
    }

    public abstract class IGraph
    {
        public List<IGraph> succs;
        public Dictionary<string, string> GraphAttr;
        public Dictionary<string, string> NodeAttr;
        public Dictionary<string, string> EdgeAttr;
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
        public bool _strict { get; set; }
        public GraphType _type { get; set; }

        public Graph(bool strict, string id, GraphType type)
        {
            _strict = strict;
            _type = type;
        }

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