﻿using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MP3_SQL_Lib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //private WindowResizer ob;
        public MainWindow()
        {
            InitializeComponent();
            //ob = new WindowResizer(this);
        }


       
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        //old way
        /*private void Resize(object sender, MouseButtonEventArgs e)
        {
            ob.resizeWindow(sender);
        }

        private void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            ob.displayResizeCursor(sender);
        }

        private void ResetCursor(object sender, MouseEventArgs e)
        {
            ob.resetCursor();
        }

        private void top_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ob.dragWindow();
        }*/
    }



    public class WindowResizer
    {
        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource hwndSource;
        Window activeWin;

        public WindowResizer(Window activeW)
        {
            activeWin = activeW as Window;

            activeWin.SourceInitialized += new EventHandler(InitializeWindowSource);
        }


        public void resetCursor()
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                activeWin.Cursor = Cursors.Arrow;
            }
        }

        public void dragWindow()
        {
            activeWin.DragMove();
        }

        private void InitializeWindowSource(object sender, EventArgs e)
        {
            hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        IntPtr retInt = IntPtr.Zero;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Debug.WriteLine("WndProc messages: " + msg.ToString());
            //
            // Check incoming window system messages
            //
            if (msg == WM_SYSCOMMAND)
            {
                //Debug.WriteLine("WndProc messages: " + msg.ToString());
            }

            return IntPtr.Zero;
        }



        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }


        public void resizeWindow(object sender)
        {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name)
            {
                case "top":
                    activeWin.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    activeWin.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    activeWin.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    activeWin.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    activeWin.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    activeWin.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }


        public void displayResizeCursor(object sender)
        {

            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name)
            {
                case "top":
                    //activeWin.Cursor = Cursors.SizeNS;
                    //activeWin.Cursor = Cursors.Cross;
                    activeWin.Cursor = Cursors.SizeAll;
                    break;
                case "bottom":
                    activeWin.Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    activeWin.Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    activeWin.Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    activeWin.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    activeWin.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }
    }
}
