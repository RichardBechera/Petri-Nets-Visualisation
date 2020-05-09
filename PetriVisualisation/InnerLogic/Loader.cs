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
            var lines = File.ReadAllLines(path);
            
            
        }
        
        
    }
}