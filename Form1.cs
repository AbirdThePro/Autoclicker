using System.Runtime.InteropServices;

namespace Autoclicker
{
    public partial class Form1 : Form
    {
        // click variables
        public static bool clicking = false;
        public static int clickspeed = 1;

        // key codes for hotkeys
        const uint KEY_ALT = 0x1;
        const uint KEY_A = 0x41;

        // hotkey id
        const int HOTKEY_ID = 0x6942;

        public Form1()
        {
            InitializeComponent();
        }

        // gets mouse_event() from user32.dll
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        // gets RegisterHotKey() from user32.dll
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // gets UnregisterHotKey() from user32.dll
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // starts clicking
        private void startClicking(bool delayClicking=true)
        {
            if (delayClicking == true)
            {
                // delays clicking
                Thread.Sleep(1000);
            }

            // starts clicking and updates button
            button1.Text = "Stop Clicking";
            clicking = true;
        }

        // stops clicking
        private void stopClicking()
        {
            // stops clicking and updates button
            button1.Text = "Start Clicking";
            clicking = false;
        }

        // click process
        private static void click()
        {
            while (true)
            {
                if (clicking == true)
                {
                    // pauses execution
                    Thread.Sleep(1000/clickspeed);

                    // mouse down
                    mouse_event(2, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    // mouse up
                    mouse_event(4, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (clicking == false)
            {
                startClicking();
            }
            else
            {
                stopClicking();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // updates click speed
            clickspeed = (int) numericUpDown1.Value;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // starts clicking process
            Task.Run(click);

            // register hotkey
            Boolean hotkeyRegistered = RegisterHotKey(
                this.Handle, // application handle
                HOTKEY_ID, // id
                KEY_ALT, // key modifier
                KEY_A // key
            );
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // unregister hotkey
            Boolean hotkeyUnregistered = UnregisterHotKey(
                this.Handle, // application handle
                HOTKEY_ID // id
            );
        }

        protected override void WndProc(ref Message m)
        {
            // handled hotkey press
            if (m.Msg == 0x312)
            {
                int id = m.WParam.ToInt32();

                if (id == HOTKEY_ID)
                {
                    if (clicking == false)
                    {
                        // instantly starts clicking
                        startClicking(false);
                    }
                    else
                    {
                        stopClicking();
                    }
                }
            }

            base.WndProc(ref m);
        }
    }
}