namespace PetriVisualisation.Graph_Algorithms
{
    //! delete later
    public class Typecheck<TFrom, TTo>
    {
        
            public bool CanConvert { get; private set; }

            public Typecheck(TFrom from)
            {
                try
                {
                    TTo to = (TTo)(dynamic)from;
                    CanConvert = true;
                }
                catch
                {
                    CanConvert = false;
                }
            }
        
    }
}