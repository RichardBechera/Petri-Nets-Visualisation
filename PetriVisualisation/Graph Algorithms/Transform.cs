using System;
using System.Collections.Generic;
using System.Linq;
using PetriVisualisation.LoadInnerLogic;

namespace PetriVisualisation.Graph_Algorithms
{
    public class Transform
    {
        public static Graph transformGraph(LoadInnerLogic.Graph graph) => CreateGraph(graph);

        public static Graph transformTransposeGraph(LoadInnerLogic.Graph graph) => CreateGraph(graph, true);

        private static Graph CreateGraph(LoadInnerLogic.Graph graph, bool transpose = false)
        {
            var newGraph = new Graph(graph.Type,
                new Attributes(graph.id, graph.id, graph.GraphAttr, graph.NodeAttr, graph.EdgeAttr), graph.strict)
            {
                subgraphs = transformSubgraphs(graph), nodes = transformNodes(graph, transpose)
            };

            return newGraph;
        }

        private static List<Attributes> transformSubgraphs(LoadInnerLogic.Graph graph)
        {
            return graph.onlySubgraphs().ConvertAll(sub => new Attributes()
            {
                belonging = sub.belonging,
                EdgeAttr = sub.EdgeAttr,
                GraphAttr = sub.GraphAttr,
                NodeAttr = sub.NodeAttr,
                id = sub.id
            });
        }

        private static List<Node> transformNodes(LoadInnerLogic.Graph graph, bool transpose)
        {
            var nodes = newNodes(graph.onlyNodes()); //graph succs ?
            var edges = !transpose ? graph.edges : transposeEdges(graph.edges);
            nodes.ForEach(node => node.succs = outgoingEdges(node.attr.id, edges)
                    .ConvertAll(edge => 
                        new System.Tuple<Node, Dictionary<string, string>>(findNode(edge.tailId, nodes), edge.edgeAttr)));
            return nodes;
        }

        private static List<Edge> transposeEdges(List<Edge> edges) => edges
            .ConvertAll(edge => new Edge()
            {
                headId = edge.tailId,
                tailId = edge.headId,
                edgeAttr = edge.edgeAttr
            });
        

        private static List<Edge> outgoingEdges(string origin, List<Edge> edges) => edges
            .Where(edge => edge.headId.Equals(origin))
            .ToList();

        private static Node findNode(string node, List<Node> nodes) => nodes
            .Find(x => x.attr.id.Equals(node));

        private static List<Node> newNodes(List<DotNode> nodes) => nodes
            .ConvertAll(node => new Node()
            {
                attr = new Attributes(node.id, node.belonging, node.GraphAttr, node.NodeAttr, node.EdgeAttr)
            });
        
    }
}