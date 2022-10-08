using Magification.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UlraWide_ScreenShare
{
    public class ResizeHelper
    {

        Window _window;
        public ResizeHelper(Window window)
        {
            _window = window;
        }

        public static double GetWidth(DragDeltaDirection direction, System.Windows.Controls.Primitives.DragDeltaEventArgs e) => direction switch
        {
            DragDeltaDirection.W => -e.HorizontalChange,
            DragDeltaDirection.SW => -e.HorizontalChange,
            DragDeltaDirection.NW => -e.HorizontalChange,

            DragDeltaDirection.E => e.HorizontalChange,
            DragDeltaDirection.SE => e.HorizontalChange,
            DragDeltaDirection.NE => e.HorizontalChange,
            _ => 0,
        };

        public static double GetHeight(DragDeltaDirection direction, System.Windows.Controls.Primitives.DragDeltaEventArgs e) => direction switch
        {
            DragDeltaDirection.N => -e.VerticalChange,
            DragDeltaDirection.NE => -e.VerticalChange,
            DragDeltaDirection.NW => -e.VerticalChange,

            DragDeltaDirection.S => e.VerticalChange,
            DragDeltaDirection.SE => e.VerticalChange,
            DragDeltaDirection.SW => e.VerticalChange,
            _ => 0,
        };

        public static double GetTop(DragDeltaDirection direction, System.Windows.Controls.Primitives.DragDeltaEventArgs e) => direction switch
        {
            DragDeltaDirection.N => e.VerticalChange,
            DragDeltaDirection.NE => e.VerticalChange,
            DragDeltaDirection.NW => e.VerticalChange,
            _ => 0,
        };

        public static double GetLeft(DragDeltaDirection direction, System.Windows.Controls.Primitives.DragDeltaEventArgs e) => direction switch
        {
            DragDeltaDirection.W => e.HorizontalChange,
            DragDeltaDirection.SW => e.HorizontalChange,
            DragDeltaDirection.NW => e.HorizontalChange,
            _ => 0,
        };

        public void Resize(DragDeltaDirection direction, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            using (var d = _window.Dispatcher.DisableProcessing())
            {
                var width = _window.Width + GetWidth(direction, e);
                var height = _window.Height + GetHeight(direction, e);
                var top = _window.Top + GetTop(direction, e);
                var left = _window.Left + GetLeft(direction, e);
                if (width >= _window.MinWidth && height >= _window.MinHeight)
                {
                    _window.Top = top;
                    _window.Left = left;
                    _window.Width = width;
                    _window.Height = height;
                    
                }

            }
        }
    }
}

