using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Diagnostics;


namespace CSGO_BombtimerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string steamPath;
        private BombWindow bombWindow = new BombWindow();
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            
            contextMenu1 = new System.Windows.Forms.ContextMenu();
            menuItem1 = new System.Windows.Forms.MenuItem();
            contextMenu1.MenuItems.AddRange(
            new System.Windows.Forms.MenuItem[] { this.menuItem1 });

            menuItem1.Text = "E&xit";
            menuItem1.Click += new EventHandler(menuItem1_Click);
            ni.ContextMenu = contextMenu1;

            ni.Icon = Properties.Resources.c4icon;
            ni.Visible = false;
            ni.BalloonTipText = "Minimized to tray";
            ni.BalloonTipTitle = "BombTimer 2.0";
            ni.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            bombWindow.Show();


        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                ni.Visible = true;
                ni.ShowBalloonTip(3000);
            }
            else if(WindowState == WindowState.Normal)
            {
                this.Show();
               // this.Activate();
                ni.Visible = false;
            }

            base.OnStateChanged(e);
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);

        }


        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);

        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey steamkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");

            if (steamkey != null)
            {
                steamPath = steamkey.GetValue("installPath").ToString() + "\\steam.exe";
            }
            Process.Start(@steamPath, "-applaunch 730");
        }

        private void label5_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/Crapy/CSGOBombTimer");
            }
            catch { }
        }

        private void label5_MouseEnter(object sender, MouseEventArgs e)
        {
            label5.Foreground=new SolidColorBrush(Colors.Navy);
            Cursor = Cursors.Hand;
        }

        private void label5_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            label5.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5C6EDE"));
        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void timerbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (timerbox.Text != "")
            {
                bombWindow.BOMB_SECS = Int32.Parse(timerbox.Text);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }


}
