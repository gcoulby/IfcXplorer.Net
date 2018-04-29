using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace IfcXplorer.CustomControls
{
    internal class IconToggleButton : ToggleButton
    {
        static IconToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconToggleButton), new FrameworkPropertyMetadata(typeof(IconToggleButton)));
        }

        public static readonly DependencyProperty ToggleIconCharProperty = DependencyProperty.Register("ToggleIconChar", typeof(char), typeof(IconToggleButton), new PropertyMetadata(default(char)));
        public char ToggleIconChar
        {
            get => (char)GetValue(ToggleIconCharProperty);
            set => SetValue(ToggleIconCharProperty, value);
        }

        public static readonly DependencyProperty ToggleOrientationCharProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(IconToggleButton), new PropertyMetadata(default(Orientation)));
        public Orientation Orientation
        {
            get => (Orientation)GetValue(ToggleIconCharProperty);
            set => SetValue(ToggleIconCharProperty, value);
        }
    }
}
