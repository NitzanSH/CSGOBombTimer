﻿using System;
using System.Timers;
using System.Windows.Forms;
using CSGSI;
using CSGSI.Nodes;
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
            Form2 frm = new Form2(this);
            frm.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
            frm.Show();
            this.Opacity = 0;

            GameStateListener gsl = new GameStateListener(3000);
            gsl.NewGameState += new NewGameStateHandler(OnNewGameState);

            //start listening on http://127.0.0.1:3000/
            if (gsl.Start())
            {
                label1.Text = "Listening..";
            }
            else
            {
                //Console.WriteLine("Error starting GSIListener.");
                label1.Text = "Error";
            }
        }
        static bool IsPlanted = false;
        private void OnNewGameState(GameState gs)
        {
            if (!IsPlanted &&
               gs.Round.Phase == RoundPhase.Live &&
               gs.Round.Bomb == BombState.Planted &&
               gs.Previously.Round.Bomb == BombState.Undefined)
            {
                timestamp = Int32.Parse(gs.Provider.TimeStamp);
                t.Enabled = true;
                IsPlanted = true;
            }
            else if(IsPlanted && (gs.Round.Phase == RoundPhase.Over || gs.Round.Phase == RoundPhase.FreezeTime))
            {
                bombTimer = 0;
                t.Enabled = false;
                this.Invoke((MethodInvoker)(() => this.Opacity = 0));
                IsPlanted = false;
            }

        }

        void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
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
