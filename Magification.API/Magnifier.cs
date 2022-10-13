using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Point = System.Windows.Point;
using Panel = System.Windows.Controls.Panel;
using System.Windows.Forms.Integration;

namespace Magification.API
{
    public class Magnifier : IDisposable
    {
        public Point LocationToMagnify { get; set; }
        private const float MAGNIFICATION = 1;
        private Window control;
        private IntPtr hwndMag;

        private bool initialized;
        private RECT magWindowRect = new RECT();

        private DispatcherTimer timer;

        public Magnifier(Window control)
        {
            this.control = control;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        ~Magnifier()
        {
            Dispose(false);
        }

        public virtual void ResizeMagnifier(Point magnifierLocation, int width, int height)
        {
            if (initialized && hwndMag != IntPtr.Zero)
            {
                HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(control);
                // Resize the control to fill the window.
                NativeMethods.SetWindowPos(hwndMag, IntPtr.Zero,
                    (int)magnifierLocation.X, (int)magnifierLocation.Y, width, height, 0);
            }
        }


        public void SetTransparentStyle()
        {
            NativeMethods.ShowWindow(hwndMag, 0);


            NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true);
        }

        public void SetNormalStyle()
        {
            NativeMethods.ShowWindow(hwndMag, 9);
            SetColorEffect();
            NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true);
        }

        public void SetAlpha(byte alpha)
        {
            NativeMethods.SetLayeredWindowAttributes(hwndMag, 0, alpha, LayeredWindowAttributeFlags.LWA_ALPHA);
        }

        public void SetColorEffect()
        {
            ColorEffect colorEffect = new ColorEffect()
            {
                transform = new float[25]
                {
                    1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                    0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
                    0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
                    0.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                    0.07f,  0.07f,  0.07f,  0.0f,  1.0f
                }
            };

            NativeMethods.MagSetColorEffect(hwndMag, ref colorEffect);
        }
        public void ResetColorEffect()
        {
            NativeMethods.MagSetColorEffect(hwndMag, IntPtr.Zero);
        }

        public virtual void UpdateMaginifier()
        {
            if (!initialized || hwndMag == IntPtr.Zero)
                return;

            RECT sourceRect = new RECT();

            sourceRect.left = (int)LocationToMagnify.X;//mousePoint.x - width / 2;
            sourceRect.top = (int)LocationToMagnify.Y;//mousePoint.y - height / 2;

            // Set the source rectangle for the magnifier control.
            NativeMethods.MagSetWindowSource(hwndMag, sourceRect);

            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(control);
            // Reclaim topmost status, to prevent unmagnified menus from remaining in view. 
            NativeMethods.SetWindowPos(hwndSource.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE);

            // Force redraw.
            NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true);
        }

        public void SetupMagnifier(Point magnifierLocation, int width, int height)
        {
            
            initialized = NativeMethods.MagInitialize();

            if (!initialized)
                return;

            IntPtr hInst;

            hInst = NativeMethods.GetModuleHandle(null);

            var hwndSource = new WindowInteropHelper(control);
            // Create a magnifier control that fills the client area.
            NativeMethods.GetClientRect(hwndSource.Handle, ref magWindowRect);
            hwndMag = NativeMethods.CreateWindow(0,
                NativeMethods.WC_MAGNIFIER,
                "MagnifierWindow", (int)WindowStyles.WS_CHILD | (int)WindowStyles.WS_VISIBLE,
                (int)magnifierLocation.X, (int)magnifierLocation.Y, width, height,
                hwndSource.Handle, IntPtr.Zero, hInst, IntPtr.Zero);

            //hwndMag = NativeMethods.CreateWindow((int)ExtendedWindowStyles.WS_EX_CLIENTEDGE, NativeMethods.WC_MAGNIFIER,
            //    "MagnifierWindow", (int)WindowStyles.WS_CHILD | (int)MagnifierStyle.MS_SHOWMAGNIFIEDCURSOR |
            //    (int)WindowStyles.WS_VISIBLE,
            //    (int)magnifierLocation.X, (int)magnifierLocation.Y, width, height, hwndSource.Handle, IntPtr.Zero, hInst, IntPtr.Zero);

            if (hwndMag == IntPtr.Zero)
            {
                return;
            }

            // Set the magnification factor.
            //Transformation matrix = new Transformation(MAGNIFICATION);
            //NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);
            //NativeMethods.SetLayeredWindowAttributes(hwndSource.Handle, 0x00009900, 0, LayeredWindowAttributeFlags.LWA_COLORKEY);
            initialized = true;
            timer.Interval = TimeSpan.FromMilliseconds(NativeMethods.USER_TIMER_MINIMUM);
            timer.Start();
    
           

        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateMaginifier();
        }

        protected void RemoveMagnifier()
        {
            if (initialized)
                NativeMethods.MagUninitialize();
        }

        public virtual void Dispose(bool disposing)
        {
            timer.Stop();
            RemoveMagnifier();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
