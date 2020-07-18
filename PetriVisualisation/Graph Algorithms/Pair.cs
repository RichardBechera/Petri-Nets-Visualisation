namespace PetriVisualisation.Graph_Algorithms
{
    public class Pair<T1, T2>
    {
        //!useless => Tuple
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}