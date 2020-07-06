using System;

namespace PetriVisualisation
{
    public class MoveRuleEventArgs : EventArgs
    {
        public Rule Rule { get; set; }
        public string Context { get; set; }
        
        public Enter Enter { get; set; }
    }
}