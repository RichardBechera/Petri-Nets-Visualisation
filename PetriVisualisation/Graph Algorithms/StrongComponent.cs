using System.Collections.Generic;

namespace PetriVisualisation.Graph_Algorithms
{
    public class StrongComponent
    {
        public List<Node> nodes = new List<Node>();
        public Node outgoing;    //lets pretend thats only one edge goes into cycle and out of cycle
        public Node incoming; //! this is more crucial, lets make it List<Node> later
        public StrongComponent outside;
        public StrongComponent inside;
            
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