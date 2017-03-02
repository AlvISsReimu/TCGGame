using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCGGame
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            Sunisoft.IrisSkin.SkinEngine skinEngine1 = new Sunisoft.IrisSkin.SkinEngine((System.ComponentModel.Component)form1);
            skinEngine1.SkinFile = Application.StartupPath + @"\MacOS.ssk";
            skinEngine1.TitleFont = new System.Drawing.Font("华文细黑", 10F);
            Application.Run(form1);
        }
    }
}