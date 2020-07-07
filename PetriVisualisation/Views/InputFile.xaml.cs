using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Input.Views
{
    public class InputFile : UserControl
    {
        public InputFile()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}