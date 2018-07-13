﻿namespace Microsoft.Windows.Interop.Sample
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media;

    using Microsoft.Windows.Interop;

    public class WrappedRectangle : WindowsXamlHost
    {
        public WrappedRectangle() : base()
        {
            base.TypeName = "Windows.UI.Xaml.Shapes.Rectangle";  

            // Rectangle's HorizontalAlignment and VerticalAlignment properties default to Stretch.
            // The control will fill all space available in the DesktopWindowXamlSource window.
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Properties set in markup need to be re-applied in OnInitialized.  
            Fill = fill;
        }

        [Browsable(false)]
        public override string TypeName
        {
            get
            {
                return base.TypeName;
            }
            set
            {
                // Don't allow setting TypeName
            }
        }


        private string fill;

        [Category("UWP XAML Rectangle")]
        public string Fill
        {
            set
            {
                fill = value;

                // UWP XAML content is not created in base.OnInitialized
                if (XamlRoot != null)
                {
                    global::Windows.UI.Xaml.Shapes.Rectangle rectangle = XamlRoot as global::Windows.UI.Xaml.Shapes.Rectangle;

                    Color wpfColor = (Color)ColorConverter.ConvertFromString(value);
                    
                    rectangle.Fill = new global::Windows.UI.Xaml.Media.SolidColorBrush(ConvertWPFColorToUWPColor(wpfColor));
                }

            }
        }

        private global::Windows.UI.Color ConvertWPFColorToUWPColor(Color wpfColor)
        {
            return global::Windows.UI.Color.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B);
        }
    }
}