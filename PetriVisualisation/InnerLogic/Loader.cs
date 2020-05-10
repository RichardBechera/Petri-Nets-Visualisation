using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using ReactiveUI;


namespace PetriVisualisation
{
    public class Loader
    {
        //TODO rework into async
        //TODO use delegate for better decomposition
        public static void LoadGraph(string path)
        {
            char nextChar;
            var buffer = "";
            var graph = new Graph(false, string.Empty, GraphType.Empty, null);
            using (var sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    if (char.IsWhiteSpace(nextChar = (char) sr.Read()))
                    {
                        if (buffer != "")
                        {
                            if (!DetermineGraph(buffer, graph))
                                return;
                            buffer = "";
                        }        
                        continue;
                    }

                    if (nextChar.Equals('{'))
                    {
                        if (buffer != "")
                        {
                            if (!DetermineGraph(buffer, graph))
                                return;
                            buffer = "";
                        }
                        LoadGraphContent(graph, sr);
                    }
                    buffer += nextChar;
                }
            }
            
        }

        private static bool LoadGraphContent(Graph graph, StreamReader sr)
        {
            var buffer = "";
            graph._content = new List<Stmt>();
            var stmt = new Stmt();
            while (sr.Peek() >= 0)
            {
                char nextChar;
                if (char.IsWhiteSpace(nextChar = (char) sr.Read()))
                {
                    if (buffer.Equals(""))
                    {
                        DetermineStmtWithoutId(buffer, out var type);
                        if (type == StmtType.AttrStmt) //TODO delete this line after differentiated between subgraphs
                        {
                            LoadStmtContent(sr, buffer, out var stmt_n, type);
                            stmt._type = type;
                            stmt._content = stmt_n;
                        }
                        buffer = "";
                    }        
                    continue;
                }

                if (nextChar.Equals('"'))
                {
                    buffer += '"';
                    if (!LoadId(sr, ref buffer))
                        return true;
                    buffer += '"';
                    if (!DetermineStmtWithId(sr, out var type, out var attr))
                        return true;
                    stmt._type = type;
                    
                    //TODO
                    continue;
                }

                /*if (nextChar.Equals('{'))
                {
                    if (buffer != "")
                    {
                        if (!DetermineGraph(buffer, graph))
                            return true;
                        buffer = "";
                    }
                    LoadGraphContent(graph, sr);
                }*/
                buffer += nextChar;
            }
            return false;
        }

        private static bool LoadId(TextReader sr, ref string buffer)
        {
            var escaped = false;
            while (sr.Peek() >= 0)
            {
                var nextChar = (char) sr.Read();

                switch (nextChar)
                {
                    case '"' when !escaped:
                        return false;
                    case '\\':
                        escaped = true;
                        break;
                    default:
                        buffer += nextChar;
                        escaped = false;
                        break;
                }
            }

            return true;
        }

        private static bool DetermineStmtWithId(StreamReader sr, out StmtType type, out bool attr)
        {
            while (sr.Peek() >= 0)
            {
                char nextChar;
                if (char.IsWhiteSpace(nextChar = (char) sr.Read()))
                    continue;
                (type, attr) = nextChar switch
                {
                    '=' => (StmtType.Id, false),
                    '-' => (StmtType.EdgeStmt, false),
                    '[' => (StmtType.NodeStmt, true),
                    ';' =>
                    //TODO if new line, default should fail    
                    (StmtType.NodeStmt, false),
                    _ => (StmtType.NodeStmt, false)
                };
                return false;
            }

            type = StmtType.Id;
            attr = false;
            return true;
        }

        private static bool DetermineStmtWithoutId(string buffed, out StmtType type)
        {
            type = buffed switch
            {
                "subgraph" => StmtType.Subgraph,
                "graph" => StmtType.AttrStmt,
                "node" => StmtType.AttrStmt,
                "edge" => StmtType.AttrStmt,
                _ => StmtType.Id //TODO this will be later on if id didnt start with "

            };
            return false; //TODO take care of parse exception later
        }

        private static bool LoadStmtContent(StreamReader sr, string buffered, out Stmt_ stmt, StmtType type)
        {
            stmt = null;
            switch (type)
            {
                case StmtType.Id:
                    //TODO later on parse ID=ID case
                    break;
                case StmtType.Subgraph:
                    break;
                case StmtType.AttrStmt:
                    var stmt_n = new AttrStmt();
                    stmt_n._attrList = new List<AList>();
                    if (!LoadAttrStmt(sr, stmt_n))
                        return true;
                    stmt = stmt_n;
                    break;
                case StmtType.EdgeStmt:
                    break;
                case StmtType.NodeStmt:
                    stmt = new NodeStmt();
                    break;
                default:
                    return true;
            }

            return false;
        }

        private static bool LoadAlist(StreamReader sr, AList alist, out bool empty)
        {
            var buffer = "";
            var first = false;
            var lookEnd = false;
            empty = true;
            while (sr.Peek() >= 0)
            {
                char nextChar;
                if (char.IsWhiteSpace(nextChar = (char) sr.Read()))
                    continue;
                if (lookEnd)
                {
                    if (!nextChar.Equals(',') || !nextChar.Equals(';'))
                        return true;
                    alist._reminder = new AList();
                    if (!LoadAlist(sr, alist._reminder, out var isempty))
                        return true;
                    if (isempty)
                        alist._reminder = null;
                    return false;

                }
                switch (nextChar)
                {
                    case '"':
                    {
                        buffer += nextChar;
                        if (!LoadId(sr, ref buffer))
                            return true;
                        buffer += '"';
                        if (!first)
                        {
                            alist._id1 = buffer;
                            first = true;
                        }
                        else
                        {
                            alist._id2 = buffer;
                            lookEnd = true;
                        }

                        buffer = "";
                        break;
                    }
                    case '=' when !first:
                        return true;
                    case ']':
                        empty = true;
                        return false;
                    default:
                        return true;
                }
            }

            return true;
        }

        
        private static bool LoadAttrStmt(StreamReader sr, AttrStmt attrStmt)
        {
            while (sr.Peek() >= 0)
            {
                if(sr.Peek().Equals(';') || sr.Peek().Equals('\n'))
                    break;
                char nextChar;
                if (char.IsWhiteSpace(nextChar = (char) sr.Read()))
                    continue;
                if (!nextChar.Equals('[')) return true;
                var alist = new AList();
                if (!LoadAlist(sr, alist, out var empty))
                    return true;
                if(!empty)
                    attrStmt._attrList.Add(alist);

            }

            return false;
        }

        private static bool DetermineGraph(string input, Graph graph)
        {
            switch (input.ToLower())
            {
                
                case "strict":
                    if (graph._strict || graph._type != GraphType.Empty || string.IsNullOrEmpty(graph._id) ||
                        graph._content != null)
                        return true;
                    graph._strict = true;
                    break;
                case "digraph":
                case "graph":
                    if (graph._type != GraphType.Empty || string.IsNullOrEmpty(graph._id) ||
                        graph._content != null)
                        return true;
                    graph._type = input.Equals("digraph") ? GraphType.Digraph : GraphType.Graph;
                    break;
                default:
                    if (graph._type == GraphType.Empty && !string.IsNullOrEmpty(graph._id) &&
                        graph._content == null || char.IsDigit(input.First()) /* //TODO || input.Any(a => <check for allowed chars in ID>)*/) return true;
                    graph._id = input;
                    break;
            }

            return false;
        }



    }
}