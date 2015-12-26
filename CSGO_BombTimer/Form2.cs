using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace CSGO_BombTimer
{
    public partial class Form2 : Form
    {
        private Form1 form1;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;

        public Form2(Form1 otherForm)
        {
            form1 = otherForm;

            this.components = new Container();
            this.contextMenu1 = new ContextMenu();
            this.menuItem1 = new MenuItem();
            this.notifyIcon = new NotifyIcon(this.components);

  
            this.contextMenu1.MenuItems.AddRange(
                        new MenuItem[] { this.menuItem1 });

            this.menuItem1.Index = 0;
            this.menuItem1.Text = "E&xit";
            menuItem1.Click += new EventHandler(menuItem1_Click);

            this.notifyIcon = new NotifyIcon(this.components);


            notifyIcon.Icon = Properties.Resources.C4_ico;

            notifyIcon.ContextMenu = this.contextMenu1;

            notifyIcon.Text = "Bomb Timer By Crapy";

            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.SizeChanged += new EventHandler(Form2_Resize);

        }
        
        private void menuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void numericTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (numericTextBox1.Text != "")
            {
                form1.BOMB_SECS = Int32.Parse(numericTextBox1.Text);
            }
        }


        private void notifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();

        }
        private void Form2_Resize(object sender, EventArgs e)
        {
            notifyIcon.BalloonTipTitle = "Bomb Timer";
            notifyIcon.BalloonTipText = "Minimized to tray";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }
    }
    

    public class NumericTextBox : TextBox
    {
        bool allowSpace = false;

        // Restricts the entry of characters to digits (including hex), the negative sign,
        // the decimal point, and editing keystrokes (backspace).
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }

            else if (e.KeyChar == '\b')
            {
                // Backspace key is OK
            }
            //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            //    {
            //     // Let the edit control handle control and alt key combinations
            //    }
            else if (this.allowSpace && e.KeyChar == ' ')
            {

            }
            else
            {
                // Swallow this invalid key and beep
                e.Handled = true;
                //    MessageBeep();
            }
        }

        public int IntValue
        {
            get
            {
                return Int32.Parse(this.Text);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return Decimal.Parse(this.Text);
            }
        }

        public bool AllowSpace
        {
            set
            {
                this.allowSpace = value;
            }

            get
            {
                return this.allowSpace;
            }
        }
    }
}
