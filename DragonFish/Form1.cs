using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DragonFish
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Clases, variables y metodos del click para el mouse
        // import the function in your class
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002, // Apretar
            LeftUp = 0x00000004,  // Soltar
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        
        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

            #region Setear fechas
            try
            {
                Process p = Process.GetProcessesByName("DRAGONFISH_Core").FirstOrDefault();
                if (p != null)
                {
                    IntPtr h = p.MainWindowHandle;
                    SetForegroundWindow(h);
                    SendKeys.SendWait("{TAB}");

                    Thread.Sleep(2000);
                    SendKeys.SendWait(textBox1.Text);

                    Thread.Sleep(2000);
                    SendKeys.SendWait("{TAB}");

                    Thread.Sleep(2000);
                    SendKeys.SendWait(textBox1.Text);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el programa: " + ex);
            }
            #endregion

            #region Click del Mouse
            SetCursorPos(1124, 517);

            Thread.Sleep(2000);

            MouseEvent((MouseEventFlags)0x00000002);
            MouseEvent((MouseEventFlags)0x00000004);

            //MessageBox.Show("Accion finzalida", "Interaccion Dragonfish");

            Application.Exit();
            Close();

            #endregion
        }
    }
}
