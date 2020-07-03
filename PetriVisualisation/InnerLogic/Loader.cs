using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using ReactiveUI;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PetriVisualisation.Parser_files;
using SharpDX.Direct3D11;


namespace PetriVisualisation
{

    public class Loader
    {
        public Graph graph;
        private Rule _rule;
        private IGraph workingBranch;
        private AttrType _attrOn = AttrType.None;
        private List<string> _idPool = new List<string>();
        
        
        //TODO method should take path to file or later add raw text or file as it is
        //TODO remake into async
        public void MyParseMethod()
        {
            ICharStream stream = CharStreams.fromPath("/home/richard/Downloads/petrinet18-02-2020_10-20-31.dot"); //random file for testing
            ITokenSource lexer = new DOTLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            DOTParser parser = new DOTParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.graph();
            KeyPrinter printer = new KeyPrinter();
            printer.RuleMoved += CheckState;
            printer.Terminal += TerminalHandler;
            ParseTreeWalker.Default.Walk(printer, tree);
            
        }
        
        private void CheckState(object sender, MoveRuleEventArgs e)
        {
            _rule = e.Rule;
            if (e.Rule == Rule.Graph && e.Enter == Enter.Enter)
            {
                graph = new Graph();
            }

            if (e.Enter == Enter.Leave)
            {
                switch (e.Rule)
                {
                    case Rule.Graph:
                        return; //TODO file end
                    case Rule.Subgraph:
                    case Rule.Node:
                        workingBranch = graph;
                        break;
                    case Rule.AttrStmt:
                        AttributeAssigner();
                        _attrOn = AttrType.None;
                        break;
                    default:
                        return;
                }
            }
            switch (e.Rule)
            {
                case Rule.Graph:
                    workingBranch = graph;
                    break;
                case Rule.Subgraph:
                    workingBranch = new Subgraph();
                    graph.succs.Add(workingBranch);
                    break;
                case Rule.Node:
                    workingBranch = new Node();
                    graph.succs.Add(workingBranch);
                    break;
            }
        }

        private void TerminalHandler(object sender, TerminalEventArgs e)
        {
            if (graph == null)
                throw new Exception(message: "Graph has to be defined");
            switch (_rule)
            {
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
                default:
                    break;
            }

        }

        private void GraphHandler(string contains)
        {
            switch (contains)
            {
                case "strict":
                    graph._strict = true;
                    break;
                case "graph":
                    graph._type = GraphType.Graph;
                    break;
                case "digraph":
                    graph._type = GraphType.Digraph;
                    break;
                default:
                    graph.id = contains;
                    break;
            }
        }

        private void IdHandler(string contains)
        {
            if (_attrOn != AttrType.None)
            {
                _idPool.Add(contains);
                return;
            }
            //TODO check if Id is in correct format
            workingBranch.id = contains;
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
                        workingBranch.GraphAttr.Add(_idPool[i], _idPool[i+1]);
                        break;
                    case AttrType.Node:
                        workingBranch.NodeAttr.Add(_idPool[i], _idPool[i+1]);
                        break;
                    case AttrType.Edge:
                        workingBranch.EdgeAttr.Add(_idPool[i], _idPool[i+1]);
                        break;
                    case AttrType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
    }
}