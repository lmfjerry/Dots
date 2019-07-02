using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Dots
{
    public partial class DotsForm : Form
    {
        public DotsForm()
        {
            InitializeComponent();
        }

        private int ClicksToStart = 0;
        private int ClickCount = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            //隱藏程式本身的視窗
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();

            MouseHook.Start();
            MouseHook.MouseLeftButtonDownAction += new EventHandler(HookEvent);

            Random rnd = new Random();
            ClicksToStart = rnd.Next(0, 300);

            SetAutoRun("Dots", Application.ExecutablePath);
        }

        public static void SetAutoRun(string keyName, string filePath)
        {
            using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"software\microsoft\windows\currentversion\run", true))
            {
                runKey.SetValue(keyName, filePath);
                runKey.Close();
            }
        }

        private void HookEvent(object sender, EventArgs e)
        {
            ClickCount++;

            if (ClickCount > ClicksToStart)
            {
                CompleteThreadUI((MouseHook.MSLLHOOKSTRUCT)sender);
            }
        }

        delegate void DelegateCompleteThread(MouseHook.MSLLHOOKSTRUCT do_hook);

        private void CompleteThreadUI(MouseHook.MSLLHOOKSTRUCT do_hook)
        {
            if (this.InvokeRequired)
            {
                DelegateCompleteThread update_comolete_ui = new DelegateCompleteThread(CompleteThreadUI);
                this.Invoke(update_comolete_ui, do_hook);
            }
            else
            {
                int hx = do_hook.pt.x, hy = do_hook.pt.y;
                //this.Text = String.Format("{0} : {1}", hx, hy);

                IntPtr desktopPtr = GetDC(IntPtr.Zero);
                using (Graphics graph = Graphics.FromHdc(desktopPtr))
                {
                    try
                    {
                        Random rnd = new Random();
                        int random_r = rnd.Next(0, 255);
                        int random_g = rnd.Next(0, 255);
                        int random_b = rnd.Next(0, 255);

                        //Color cust_red = Color.FromArgb(255, 255, 0, 0);
                        Color cust_color = Color.FromArgb(255, random_r, random_g, random_b);
                        SolidBrush brush = new SolidBrush(cust_color);

                        //g.FillRectangle(b, new Rectangle(hx, hy, 10, 10));
                        graph.FillPie(brush, new Rectangle(hx, hy, ClickCount, ClickCount), 0, 360);
                    }
                    catch (Exception ex)
                    { }
                }
                //g.Dispose();

                ReleaseDC(IntPtr.Zero, desktopPtr);

            }
        }

        #region DLL Import

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        #endregion
    }
}
