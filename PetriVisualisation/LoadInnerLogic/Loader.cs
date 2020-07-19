using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PetriVisualisation.Parser_files;

namespace PetriVisualisation.LoadInnerLogic
{

    public class Loader
    {
        private Graph _graph;
        private Rule _rule;
        private readonly Stack<IGraph> _workingBranch = new Stack<IGraph>();
        private AttrType _attrOn = AttrType.None;
        private readonly List<string> _idPool = new List<string>();
        private bool _edgeRequired;
        private readonly List<string> _edgePool = new List<string>();
        
        
        //TODO method should take path to file or later add raw text or file as it is
        //TODO remake into async
        public Graph LoadGraph(string path)
        {
            MyParseMethod(path);
            return _graph;
        }

        public async Task<Graph> LoadGraphAsync(string path)
        {
            return await Task.Run(() => LoadGraph(path));
        }
        
        private void MyParseMethod(string path)
        {
            var stream = CharStreams.fromPath(path); 
            ITokenSource lexer = new DOTLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new DOTParser(tokens) {BuildParseTree = true};
            IParseTree tree = parser.graph();
            var printer = new KeyPrinter();
            printer.RuleMoved += CheckState;
            printer.Terminal += TerminalHandler;
            ParseTreeWalker.Default.Walk(printer, tree);
        }

        private void CheckState(object sender, MoveRuleEventArgs e)
        {
            _rule = e.Rule;
            if (e.Rule == Rule.Graph && e.Enter == Enter.Enter)
            {
                _graph = new Graph();
            }

            if (e.Enter == Enter.Leave)
            {
                switch (e.Rule)
                {
                    case Rule.Graph:
                        _workingBranch.Pop();
                        return; //TODO file end
                    case Rule.Subgraph:
                    case Rule.Node:
                        _workingBranch.Pop();
                        break;
                    case Rule.AttrStmt:
                        AttributeAssigner();
                        _attrOn = AttrType.None;
                        _idPool.Clear();
                        break;
                    case Rule.EdgeStmt:
                        _edgeRequired = false;
                        _attrOn = AttrType.None;
                        SetEdgeFromPool();
                        _edgePool.Clear();
                        break;
                }
                return;
            }
            switch (e.Rule)
            {
                case Rule.Graph:
                    _workingBranch.Push(_graph);
                    break;
                case Rule.Subgraph:
                    _workingBranch.Push(new Subgraph());
                    _graph.subgraphs.Add(_workingBranch.Peek());
                    break;
                case Rule.Node:
                    var node = new DotNode();
                    node.belonging = _workingBranch.Peek().id;
                    _workingBranch.Push(node);
                    _graph.succs.Add(_workingBranch.Peek());
                    break;
                case Rule.EdgeStmt:
                    _edgeRequired = true;
                    _graph.edges.Add(new Edge());
                    break;
                case Rule.Alist:
                    if (_edgeRequired)
                    {
                        _edgeRequired = false;
                        _attrOn = AttrType.EdgeStmt;
                    }
                    break;
            }
        }

        private void TerminalHandler(object sender, TerminalEventArgs e)
        {
            if (_graph == null)
                throw new Exception(message: "Graph has to be defined");
            switch (_rule)
            {
                //TODO unused are to be implemented later, now working with simple graphs
                case Rule.Graph:
                    GraphHandler(e.Contains);
                    break;
                case Rule.Node:
                    break;
                case Rule.Subgraph:
                    break;
                case Rule.Id:
                    IdHandler(e.Contains);
                    break;
                case Rule.Port:
                    break;
                case Rule.AttrList:
                    if (_attrOn == AttrType.EdgeStmt)
                    {
                        AttrListLeaveHandler();
                    }
                    //TODO if edge on turn off else ignore
                    break;
                case Rule.AttrStmt:
                    AttributeHandler(e.Contains);
                    break;
                case Rule.Alist:
                    break;
                case Rule.EdgeRhs:
                    break;
                case Rule.EdgeStmt:
                    break;
            }

        }

        private void GraphHandler(string contains)
        {
            switch (contains)
            {
                case "strict":
                    _graph.strict = true;
                    break;
                case "graph":
                    _graph.Type = GraphType.Graph;
                    break;
                case "digraph":
                    _graph.Type = GraphType.Digraph;
                    break;
                default:
                    _graph.id = contains;
                    break;
            }
        }

        private void IdHandler(string contains)
        {
            if (_edgeRequired)
            {
                _edgePool.Add(contains);
                return;
            }
            if (_attrOn != AttrType.None)
            {
                _idPool.Add(contains);
                return;
            }
            //TODO check if Id is in correct format
            _workingBranch.Peek().id = contains;
        }

        private void AttributeHandler(string contains)
        {
            _attrOn = contains.ToLower() switch
            {
                "graph" => AttrType.Graph,
                "node" => AttrType.Node,
                "edge" => AttrType.Edge,
                _ => AttrType.None //! this should not happen, I can assume there will be nothing else
            };

        }

        private void AttributeAssigner()
        {
            if (_idPool.Count % 2 != 0)
                throw new Exception(message:"Wrong attribute format");
            for (var i = 0; i < _idPool.Count; i += 2)
            {
                switch (_attrOn)
                {
                    case AttrType.Graph:
                        _workingBranch.Peek().GraphAttr.Add(_idPool[i], _idPool[i+1]);
                        break;
                    case AttrType.Node:
                        _workingBranch.Peek().NodeAttr.Add(_idPool[i], _idPool[i+1]);
                        break;
                    case AttrType.Edge:
                        _workingBranch.Peek().EdgeAttr.Add(_idPool[i], _idPool[i+1]);
                        break;
                    case AttrType.None:
                    case AttrType.EdgeStmt:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void AttrListLeaveHandler()
        {
            if (_edgePool.Count % 2 != 0)
                throw new Exception(message:"Wrong attribute format");
            for (var i = 0; i < _edgePool.Count; i += 2)
            {
                _graph.edges.Last().edgeAttr.Add(_edgePool[i], _edgePool[i+1]);
            }
        }

        private void SetEdgeFromPool()
        {
            if (_edgePool.Count < 2)
            {
                throw new Exception(message:"Edge needs at least 2 vertexes");
            }
            
            _graph.edges.Last().headId = _edgePool[0];
            _graph.edges.Last().tailId = _edgePool[1];
            //TODO later implement if more vertexes in one row
        }
        
    }
}