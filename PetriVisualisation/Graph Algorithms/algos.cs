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
        
        public static List<StrongComponent> getTopoOnScc(PetriVisualisation.Graph graph, Graph graph_n = null)
        {
            var graphN = graph_n ?? Transform.transformGraph(graph);
            var graphT = Transform.transformTransposeGraph(graph);
            return topoOnScc(StrongComponents(graphN, graphT));
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

        public List<Tuple<Node, int, int>> SccOrdering(StrongComponent component)
        {
            if (component.nodes.Count <= 1)
                return component.nodes.Select(nd => new Tuple<Node, int, int>(nd, 0, 1)).ToList();
            var ordering = SccBfsOrdering(component);
            var s = ordering.First().Item2;
            var t = ordering
                .Aggregate(ordering.First().Item2, (res, cur) => res > cur.Item2 ? res : cur.Item2);
            var left = ordering.Skip(1).TakeWhile(a => a.Item2 < t).ToList();
            var right = ordering.SkipWhile(a => a.Item2 <= t).ToList();
            var addition = 0;
            if (right.Last().Item2 < 1)
                addition = -right.Last().Item2 + 1;

            return right
                .Select(a => new Tuple<Node, int, int>(a.Item1, a.Item2 + addition, 2))
                .Concat(left.Select(a => new Tuple<Node, int, int>(a.Item1, a.Item2, 0)))
                .Append(ordering.Take(1).Select(a => new Tuple<Node, int, int>(a.Item1, a.Item2, 1)).First())
                .Append(ordering.SkipWhile(a => a.Item2 < t).Take(1)
                    .Select(a => new Tuple<Node, int, int>(a.Item1, a.Item2, 1)).First())
                .OrderBy(a => a.Item1).ToList();
        }

        private static List<Tuple<Node, int>> SccBfsOrdering(StrongComponent component)
        {
            //! only 1 outgoing, only 1 ingoing
            //! in general extremely bad approach causing many bugs => t doesnt have to be the only node on its layer found by bfs
            
            var s = component.incoming.Count == 0 ? component.nodes.First() : component.incoming.First(); //!assuming they are already sorted (p0 < p1 < p2 < ...), and well, assuming there is only 1
            var t = component.outgoing.Count == 0 ? component.nodes.Last() : component.outgoing.Last();
            var visited = component.nodes.ToDictionary(c => c, v => false);
            visited[s] = true;
            var order = new List<Tuple<Node, int>> {new Tuple<Node, int>(s, 0)};
            var queue = new Queue<Tuple<Node, int>>();
            queue.Enqueue(new Tuple<Node, int>(s, 0));
            while (queue.Count > 0)
            {
                var (node, ord) = queue.Dequeue();
                if (component.outgoing.Contains(node))
                {
                    visited[node] = true;
                    order.Add(new Tuple<Node, int>(node, ord+1));
                    continue;
                }

                foreach (var succ in node.succs
                    .Where(nd => component.nodes.Contains(nd.Item1))
                    .Select(tup => tup.Item1).Where(nd=> !visited[nd]))
                {
                    order.Add(new Tuple<Node, int>(succ, ord+1));
                    queue.Enqueue(order.Last());
                    visited[succ] = true;
                }
            }
            queue.Enqueue(order.Find(o => o.Item1.Equals(t)));
            while (queue.Count > 0)
            {
                var (node, ord) = queue.Dequeue();
                foreach (var succ in node.succs
                    .Where(nd => component.nodes.Contains(nd.Item1))
                    .Select(tup => tup.Item1).Where(nd=> !visited[nd]))
                {
                    order.Add(new Tuple<Node, int>(succ, ord-1));
                    queue.Enqueue(order.Last());
                    visited[succ] = true;
                }
            }

            return order;
        }

        private static List<StrongComponent> StrongComponents(Graph graph, Graph transposed)
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
                foreach (var outside in component.outside
                    .Where(outside => !outside.flag || outside.depth < depth + 1))
                {
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
                    var res = components
                        .Where(c1 => !c1.Equals(c) && c1.outgoing != null)
                        .Where(c1 => c1.nodes
                            .Exists(node => node.succs
                                .Exists(nd => c.incoming.Any(cc => nd.Item1.Equals(cc)))))
                        .ToList();

                    c.inside = res.Count > 0 ? res : null;
                }

                if (c.outgoing == null) continue;
                {
                    var res2 = components
                        .Where(c2 => !c2.Equals(c) && c2.incoming != null)
                        .Where(c2 => c2.nodes
                            .Exists(node => c.outgoing
                                .Any(k => k.succs
                                    .Exists(nd => nd.Item1.Equals(node)))))
                        .ToList();

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