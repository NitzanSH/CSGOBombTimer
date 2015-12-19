using System;
using System.Timers;
using System.Windows.Forms;
using CSGSI;
using Timer = System.Timers.Timer;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CSGO_BombTimer
{
    
    public partial class Form1 : Form
    {
        public int bombTimer = 0;
        public int BOMB_SECS = 40;
        public Timer t = new Timer(1);
        public int timestamp = 0;
        public int calculateSeconds()

        {

            TimeSpan cac = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)cac.TotalSeconds;

            return secondsSinceEpoch;

        }

        public Form1()
        {
            t.Elapsed += OnTimedEvent;
            t.AutoReset = true;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
            frm.Show();
            this.Opacity = 0;
            //this.SetDesktopLocation(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - this.Width, 100);

            //subscribe to the NewGameState event
            GSIListener.NewGameState += new EventHandler(OnNewGameState);

            //start listening on http://127.0.0.1:3000/
            if (GSIListener.Start(3000))
            {
                label1.Text = "Listening..";
            }
            else
            {
                //Console.WriteLine("Error starting GSIListener.");
                label1.Text = "Error";
            }
        }

        private void OnNewGameState(object sender, EventArgs e)
        {
            //the newest GameState object is provided as the sender
            GameState gs = (GameState)sender;

            GameStateNode gsRound = gs.Round;
            GameStateNode gsProvider = gs.Provider;
            if (gsRound.GetValue("bomb") == "planted")
            {
                if (bombTimer <= 1) { 
                    timestamp = Int32.Parse(gsProvider.GetValue("timestamp"));
                    t.Enabled = true;
                }
            }
            else
            {
                bombTimer = 0;
                t.Enabled = false;
                this.Invoke((MethodInvoker)(() => this.Opacity = 0));
            }

        }

        void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            bombTimer = ((timestamp + BOMB_SECS) - calculateSeconds()) - 1;
            if (bombTimer >= 0) {
            
              label1.Invoke((MethodInvoker)(() => label1.Text = bombTimer.ToString()));
            }
            if(GetActiveWindowTitle() == "Counter-Strike: Global Offensive")
            {
                this.Invoke((MethodInvoker)(() => this.Opacity = 50));
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                IntPtr handle = GetForegroundWindow();
                Rect CSGORect = new Rect();
                GetWindowRect(handle, ref CSGORect);
                this.Invoke((MethodInvoker)(() => this.SetDesktopLocation(CSGORect.Right - this.Width, CSGORect.Top + 100 + this.Height)));
            }
            else
            {
                this.Invoke((MethodInvoker)(() => this.Opacity = 0));
            }
        }

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        
    }
}
