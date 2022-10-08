using Gma.UserActivityMonitor;
using Magification.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserActivity;

namespace UlraWide_ScreenShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsTransparent { get; private set; }
        private ResizeHelper _resizeHelper;
        private Magnifier _magnifier;

        public MainWindow()
        {
            InitializeComponent();
            HookManager.MouseMoveExt += App_MouseMove1;
            _resizeHelper = new ResizeHelper(this);
            _magnifier = new Magnifier(this);
        }

        public double MenuBarHeight => (double)Resources["MenuBarHeight"];
        public double WindowBorderThickness => (double)Resources["BorderThickness"];
        public double HideShowBarHeight => (double)Resources["HideShowBarHeight"];

        private bool _isTopBarVisible = true;

        public Point GridStartScreenLocation => contentGrid.PointToScreen(new Point(0, 0));
        public Point GridEndScreenLocation => new Point(GridStartScreenLocation.X + contentGrid.ActualWidth, GridStartScreenLocation.Y + contentGrid.ActualHeight);
        public Point GridStartWindowLocation => new Point(WindowBorderThickness, WindowBorderThickness + HideShowBarHeight + (_isTopBarVisible ? MenuBarHeight : 0.0));
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            _magnifier.SetupMagnifier(GridStartWindowLocation, (int)contentGrid.ActualWidth, (int)contentGrid.ActualHeight);
            _magnifier.LocationToMagnify = GridStartScreenLocation;
            _magnifier.UpdateMaginifier();
        }

        private void App_MouseMove1(object sender, MouseEventExtArgs e)
        {
            if (IsTransparent)
            {
                if (e.LocationX <= GridStartScreenLocation.X || e.LocationX >= GridStartScreenLocation.X + contentGrid.ActualWidth || e.LocationY <= GridStartScreenLocation.Y || e.LocationY >= GridEndScreenLocation.Y + contentGrid.ActualHeight)
                {
                    IsTransparent = false;
                    var windowHwnd = new WindowInteropHelper(this).Handle;
                    WindowsServices.SetWindowExNotTransparent(windowHwnd);
                }
            }
        }

        private void TopBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Top = Mouse.GetPosition(this).Y;
                WindowState = WindowState.Normal;
            }
            _magnifier.SetTransparentStyle();

            DragMove();

            _magnifier.SetNormalStyle();
        }


        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _magnifier.SetNormalStyle();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _magnifier.SetTransparentStyle();
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            var windowHwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(windowHwnd);
            IsTransparent = true;
        }

        private void contentGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _magnifier.ResizeMagnifier(GridStartWindowLocation, (int)contentGrid.ActualWidth, (int)contentGrid.ActualHeight);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            _magnifier.LocationToMagnify = GridStartScreenLocation;
            _magnifier.UpdateMaginifier();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            var CurrentWindow = Window.GetWindow((DependencyObject)e.Source);
            CurrentWindow.Close();
        }

        private void ButtonMinimalize_Click(object sender, RoutedEventArgs e)
        {
            var CurrentWindow = Window.GetWindow((DependencyObject)e.Source);
            CurrentWindow.WindowState = WindowState.Minimized;
        }

        private void ButtonMiaximize_Click(object sender, RoutedEventArgs e)
        {
            var CurrentWindow = Window.GetWindow((DependencyObject)e.Source);
            if (CurrentWindow.WindowState == WindowState.Normal)
            {
                var LastMaxHeight = CurrentWindow.MaxHeight;
                var LastMaxWidth = CurrentWindow.MaxWidth;

                CurrentWindow.WindowState = WindowState.Maximized;
                CurrentWindow.MaxHeight = LastMaxHeight;
                CurrentWindow.MaxWidth = LastMaxWidth;

            }
            else if (CurrentWindow.WindowState == WindowState.Maximized)
            {
                CurrentWindow.WindowState = WindowState.Normal;
            }
        }
        private void ThumbE_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.E, e);
        private void ThumbW_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.W, e);
        private void ThumbS_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.S, e);
        private void ThumbN_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.N, e);
        private void ThumbNE_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.NE, e);
        private void ThumbNW_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.NW, e);
        private void ThumbSE_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.SE, e);
        private void ThumbSW_DragDelta(object sender, DragDeltaEventArgs e) => _resizeHelper.Resize(DragDeltaDirection.SW, e);


        private void ArrowUpButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //_isTopBarVisible = false;
            //_magnifier.LocationToMagnify = GridStartScreenLocation;
            //_magnifier.UpdateMaginifier();
            //_magnifier.ResizeMagnifier(GridStartWindowLocation, (int)contentGrid.ActualWidth, (int)contentGrid.ActualHeight);
        }

        private void ArrowDownButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //_isTopBarVisible = true;
            //_magnifier.LocationToMagnify = GridStartScreenLocation;
            //_magnifier.UpdateMaginifier();
            //_magnifier.ResizeMagnifier(GridStartWindowLocation, (int)contentGrid.ActualWidth, (int)contentGrid.ActualHeight);
        }


        private void ArrowUpButtonClickStoryBoard_Completed(object sender, EventArgs e)
        {
            _isTopBarVisible = false;
            _magnifier.LocationToMagnify = GridStartScreenLocation;
            _magnifier.UpdateMaginifier();
            _magnifier.ResizeMagnifier(GridStartWindowLocation, (int)contentGrid.ActualWidth, (int)contentGrid.ActualHeight);
        }
    }

}
