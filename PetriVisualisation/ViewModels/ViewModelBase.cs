using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using ReactiveUI;

namespace PetriVisualisation.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        private string path = String.Empty;
        
        public string Path
        {
               get => path;
               set => this.RaiseAndSetIfChanged(ref path, value);            
        }
        
        private string preview = "Preview of choosen file";
        
        public string Preview
        {
            get => preview;
            set => this.RaiseAndSetIfChanged(ref preview, value);
        }
    }
}
