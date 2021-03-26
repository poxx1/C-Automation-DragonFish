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

            mouse_event((int)value, position.X, position.Y, 0, 0);
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
            #region Obtener posicion de la ventana
            //  SetCursorPos(890, 384); >> Posicion del boton centrado
            int x, y;
            int botonX = 10, botonY = 10;
            var posicionVentana = ventana();
            y = posicionVentana.Top + botonY; //A la posicion actual le sumo el Alto
            x = posicionVentana.Left + botonX; //A la posicion actual le sumo el Ancho

            #endregion

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

                    //Thread.Sleep(2000);
                    //SendKeys.SendWait("{TAB}");

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
            SetCursorPos(x, y);

            Thread.Sleep(2000);

            MouseEvent((MouseEventFlags)0x00000002);
            MouseEvent((MouseEventFlags)0x00000004);

            Application.Exit();
            Close();
            #endregion
        }

        #region Encontrar ventana
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly,string lpWindowName);

        // Define the SetWindowPos API function.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd,IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,SetWindowPosFlags uFlags);

        // Define the SetWindowPosFlags enumeration.
        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            SynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        public void PosicionarVentana()
        {
            // Get the target window's handle.
            IntPtr target_hwnd =
            FindWindowByCaption(IntPtr.Zero, "Listado - RETAIL - SAP (Seguimiento de stock por comprobante)");
            
            if (target_hwnd == IntPtr.Zero)
            {
                MessageBox.Show( "No se encontro la ventana");
                return;
            }
            // Set the window's position.
            int width = 927;
            int height = 396;
            int x = 0;
            int y = 0;

            SetWindowPos(target_hwnd, IntPtr.Zero,x, y, width, height, 0);
        }
        #endregion

        #region Tomar posicion de la ventana
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        public Rect ventana()
        {
            Process[] processes = Process.GetProcessesByName("DRAGONFISH_Core");
            Process lol = processes[0];
            IntPtr ptr = lol.MainWindowHandle;
            Rect ventanaActual = new Rect();
            var position = GetWindowRect(ptr, ref ventanaActual);
            //MessageBox.Show(ventanaActual.Top.ToString() + ventanaActual.Left.ToString(), "DF Automation");
            return ventanaActual;
        }

        #endregion
    }
}
