using System.Windows;
using System.Windows.Controls;

namespace IfcXplorer.CustomControls
{
    internal class IconButton : Button
    {
        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public static readonly DependencyProperty IconCharProperty = DependencyProperty.Register("IconChar", typeof(char), typeof(IconButton), new PropertyMetadata(default(char)));
        public char IconChar
        {
            get => (char)GetValue(IconCharProperty);
            set => SetValue(IconCharProperty, value);
        }

        public static readonly DependencyProperty OrientationCharProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(IconButton), new PropertyMetadata(default(Orientation)));
        public Orientation Orientation
        {
            get => (Orientation)GetValue(IconCharProperty);
            set => SetValue(IconCharProperty, value);
        }
    }
}
