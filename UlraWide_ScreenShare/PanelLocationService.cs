using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UlraWide_ScreenShare
{
    public class PanelLocationService
    {
        private static Panel _panel;
        private static Window _parentWindow;
        private static  bool _initialized = false;

        public static void Initialize(Panel panel, Window parentWindow)
        {
            _panel = panel; 
            _parentWindow = parentWindow;
            _initialized = true;
        }
    }
}
