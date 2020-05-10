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
        public static void LoadGraph(string path)
        {
            char nextChar;
            var buffer = "";
            var graph = new Graph(false, string.Empty, GraphType.Empty, null);
            using (StreamReader sr = new StreamReader(path))
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
                        LoadGraphContent(graph, sr);
                    }
                    buffer += nextChar;
                }
            }
            
        }

        private static void LoadGraphContent(Graph graph, StreamReader sr)
        {
            
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