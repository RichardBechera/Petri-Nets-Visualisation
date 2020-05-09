using System.IO;
using System.Reactive.Linq;
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
            var graph = new Graph();
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    if (char.IsWhiteSpace(nextChar = (char) sr.Read()))
                        //TODO        
                        continue;
                    buffer += nextChar;
                }
            }
            
        }
        
        
        
        
        
    }
}