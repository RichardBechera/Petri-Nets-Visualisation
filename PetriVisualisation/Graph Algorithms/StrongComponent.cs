using System.Collections.Generic;

namespace PetriVisualisation.Graph_Algorithms
{
    public class StrongComponent
    {
        public List<Node> nodes = new List<Node>();
        public List<Node> outgoing;    //lets pretend there is only one edge goes into cycle and out of cycle
        public List<Node> incoming; 
        public List<StrongComponent> outside;
        public List<StrongComponent> inside;
        public int depth = 0;
            
        public bool flag;    //just for convenience

        public StrongComponent(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public StrongComponent()
        {
        }
    }
}