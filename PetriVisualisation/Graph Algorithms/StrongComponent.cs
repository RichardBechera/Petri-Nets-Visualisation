using System.Collections.Generic;

namespace PetriVisualisation.Graph_Algorithms
{
    public class StrongComponent
    {
        public List<Node> nodes = new List<Node>();
        public List<Node> outgoing;    //lets pretend thats only one edge goes into cycle and out of cycle
        public List<Node> incoming; //! this is more crucial, lets make it List<Node> later
        public List<StrongComponent> outside;
        public List<StrongComponent> inside;
            
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