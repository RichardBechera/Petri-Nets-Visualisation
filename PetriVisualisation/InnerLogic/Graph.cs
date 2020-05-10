using System.Collections.Generic;

namespace PetriVisualisation
{
    
    //TODO Type enums merge into less enums, move into separate folders/files
    public enum GraphType
    {
        Digraph,
        Graph,
        Empty
    }

    public enum StmtType
    {
        NodeStmt,
        EdgeStmt,
        AttrStmt,
        Id,
        Subgraph
    }

    public enum EdgeType
    {
        NodeId,
        Subgraph
    }

    public enum AttrType
    {
        Graph,
        Node,
        Edge
    }

    public enum EdgePart
    {
        NodeId,
        Subgraph
    }
    
    public class Graph
    {
        public bool _strict { get; set; }
        public string _id { get; set; }
        public List<Stmt> _content { get; set; }
        public GraphType _type { get; set; }

        public Graph(bool strict, string id, GraphType type, List<Stmt> content)
        {
            _strict = strict;
            _id = id;
            _type = type;
            _content = content;
        }

    }

    //TODO delete stmt class and use only interface instead
    public class Stmt
    {
        public StmtType _type;
        public Stmt_ _content;
    }

    public interface Stmt_
    {
    }

    public class Subgraph : Stmt_
    {
        private string _id;
        private List<Stmt> _content;
    }

    public class NodeStmt : Stmt_
    {
        private NodeId node;
        private string attr = null;
    }
    
    public class NodeId
    {
        private string _id;
        private string _port = null;
    }

    public class AttrStmt : Stmt_
    {
        private AttrType _type;
        public List<AList> _attrList;
    }

    public class AList
    {
        public string _id1;
        public string _id2;
        public AList _reminder = null;
        public bool hasNext = false;
    }

    public class EdgeStmt : Stmt_
    {
        private EdgeType _type;
        private EdgeRHS _rhs;
        private List<AList> _attrLists = null;
    }

    public class EdgeRHS
    {
        private bool _directed;
        private EdgeType _type;
        private EdgeRHS _rhs = null;
        public bool hasNext = false;
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