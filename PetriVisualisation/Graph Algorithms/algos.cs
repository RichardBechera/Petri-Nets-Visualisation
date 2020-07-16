using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Antlr4.Runtime.Dfa;
using Avalonia.Input;

namespace PetriVisualisation.Graph_Algorithms
{
    public class algos
    {
        public static List<StrongComponent> sortComponentTopology(List<StrongComponent> components) =>
            components.OrderBy(c => c.depth).ToList();
        
        public static List<StrongComponent> getTopoOnScc(PetriVisualisation.Graph graph)
        {
            var graphN = Transform.transformGraph(graph);
            var graphT = Transform.transformTransposeGraph(graph);
            return topoOnScc(strongComponents(graphN, graphT));
        }

        private static List<Node> predecessors(Node hero, List<Node> nodes) => nodes
            .Where(node => node.succs
                .Exists(nd => nd.Item1
                    .Equals(hero)))
            .ToList();

        public static int heightOfNet(List<StrongComponent> components)
        {
            var sortedComponents = sortComponentTopology(components);
            //TODO now without complex cycles, think of bigger sc
            var height = 0;
            var currentDepth = -1;
            var currentHeight = 0;
            foreach (var c in sortedComponents)
            {
                if (c.depth == currentDepth)
                {
                    currentHeight = c.nodes.Count > currentHeight ? c.nodes.Count : currentHeight;
                    continue;
                }

                height += currentHeight;    //there is only one ending node
                currentDepth = c.depth;
                currentHeight = c.nodes.Count;
            }

            return height;
        }

        public static int widthOfNet(List<StrongComponent> components)
        {
            var sortedComponents = sortComponentTopology(components);
            //TODO now without complex cycles, think of bigger sc
            var width = 1;
            var currentC = 0;
            var currentW = 0;
            foreach (var c in sortedComponents.Select(sc => sc.depth))
            {
                if (c != currentC)
                {
                    currentW = 0;
                    currentC = c;
                }

                currentW++;
                if (width < currentW)
                    width = currentW;
            }

            return width;
        }

        private static List<StrongComponent> strongComponents(Graph graph, Graph transposed)
        {
            var stack = new Stack<Node>();
            var visited = graph.nodes
                .ConvertAll(node => new {node.attr.id, pair = new Pair<Node, bool>(node, false)})
                .ToDictionary(a => a.id, a => a.pair);
            dfs(graph.nodes[0].attr.id, visited, stack); //!what if 0 nodes ?
            visited = transposed.nodes
                .ConvertAll(node => new {node.attr.id, pair = new Pair<Node, bool>(node, false)})
                .ToDictionary(a => a.id, a => a.pair);
            var components = new List<StrongComponent>();
            while (stack.Count > 0)
            {
                var scc = new List<Node>();
                var node = stack.Pop();
                if (!visited[node.attr.id].Second)
                {
                    componentDfs(node.attr.id, visited, scc, graph.nodes);
                }
                components.Add(new StrongComponent(scc));
            }
            fillComponents(graph.nodes, components);
            return components;
        }

        private static List<StrongComponent> topoOnScc(List<StrongComponent> components)
        {
            var stack = new Stack<StrongComponent>();
            components.ForEach(c => c.flag = false);
            var start = components.First(c => c.incoming == null);
            topoOnPetriGraph(stack, start, 0);
            return stack.ToList();
        }


        private static void topoOnPetriGraph(Stack<StrongComponent> stack, StrongComponent component, int depth)
        {
            component.flag = true;
            component.depth = depth;
            if (component.outside != null)
                foreach (var outside in component.outside)
                {
                    if (!outside.flag || outside.depth < depth + 1)
                        topoOnPetriGraph(stack, outside, depth + 1);
                }
                
            stack.Push(component);
        }
        

        private static void fillComponents(List<Node> nodes, List<StrongComponent> components)
        {
            components.ForEach(c => findInAndOut(c, nodes.Except(c.nodes).ToList())); 
            foreach (var c in components)
            {
                if (c.incoming != null)
                {
                    var res = new List<StrongComponent>();
                    foreach (var c1 in components.Where(c1 => !c1.Equals(c) && c1.outgoing != null).Where(c1 => c1.nodes.Exists(node => node.succs
                        .Exists(nd => nd.Item1.Equals(c.incoming)))))
                    {
                        res.Add(c1);
                    }

                    c.inside = res.Count > 0 ? res : null;
                }
               
                var res2 = new List<StrongComponent>();
                if (c.outgoing == null) continue;
                {
                    foreach (var c2 in components.Where(c2 => !c2.Equals(c) && c2.incoming != null).Where(c2 => c2.nodes.Exists(node => c.outgoing.Any(k => k.succs.Exists(nd => nd.Item1.Equals(node))))))
                    {
                        res2.Add(c2);
                    }

                    c.outside = res2.Count > 0 ? res2 : null;
                }
            }
        }

        private static void findInAndOut(StrongComponent c, List<Node> nodes)
        {
            var res = new List<Node>();
            var res2 = new List<Node>();
            foreach (var cNode in c.nodes)
            {
                if (nodes.Exists(node => node.succs.Exists(nd => nd.Item1.Equals(cNode))))
                {
                    res.Add(cNode);
                }

                if (cNode.succs.Exists(nd => nodes.Exists(node => node.Equals(nd.Item1))))
                {
                    res2.Add(cNode);
                }
            }

            c.incoming = res.Count > 0 ? res : null;
            c.outgoing = res2.Count > 0 ? res2 : null;
        }

        private static void componentDfs(string id, IReadOnlyDictionary<string, Pair<Node, bool>> visited, List<Node> components, List<Node> nodes)
        {
            var current = visited.First(x => x.Key.Equals(id)).Value;
            current.Second = true;
            foreach (var succesor in current.First.succs
                .Where(succesor => !visited[succesor.Item1.attr.id].Second))
            {
                componentDfs(succesor.Item1.attr.id, visited, components, nodes);
            }
            components.Add(nodes.Find(node => node.attr.id.Equals(id)));
        }

        private static void dfs(string id, IReadOnlyDictionary<string, Pair<Node, bool>> visited, Stack<Node> stack)
        {
            var current = visited.First(x => x.Key.Equals(id)).Value;
            current.Second = true;
            foreach (var succesor in current.First.succs
                .Where(succesor => !visited[succesor.Item1.attr.id].Second))
            {
                dfs(succesor.Item1.attr.id, visited, stack);
            }
            stack.Push(current.First);
        }
    }
}