using System;
using System.Collections.Generic;
using System.Linq;

namespace PetriVisualisation.Graph_Algorithms
{
    public class Transform
    {
        public static Graph transformGraph(PetriVisualisation.Graph graph) => CreateGraph(graph);

        private static Graph CreateGraph(PetriVisualisation.Graph graph)
        {
            var newGraph = new Graph(graph._type,
                new Attributes(graph.id, graph.id, graph.GraphAttr, graph.NodeAttr, graph.EdgeAttr), graph._strict)
            {
                subgraphs = transformSubgraphs(graph), nodes = transformNodes(graph)
            };

            return newGraph;
        }

        private static List<Attributes> transformSubgraphs(PetriVisualisation.Graph graph)
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

        private static List<Node> transformNodes(PetriVisualisation.Graph graph)
        {
            var nodes = newNodes(graph.onlyNodes()); //graph succs ?
            var edges = graph.edges;
            nodes.ForEach(node => node.succs = outgoingEdges(node.attr.id, edges)
                    .ConvertAll(edge => 
                        new Tuple<Node, Dictionary<string, string>>(findNode(edge.tailId, nodes), edge.EdgeAttr)));
            return nodes;
        }

        private static List<Edge> outgoingEdges(string origin, List<Edge> edges) => edges
            .Where(edge => edge.headId.Equals(origin))
            .ToList();

        private static Node findNode(string node, List<Node> nodes) => nodes
            .Find(x => x.attr.id.Equals(node));

        private static List<Node> newNodes(List<PetriVisualisation.Node> nodes) => nodes
            .ConvertAll(node => new Node()
            {
                attr = new Attributes(node.id, node.belonging, node.GraphAttr, node.NodeAttr, node.EdgeAttr)
            });
        
    }
}