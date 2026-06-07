using System;
using System.Windows.Forms;
using ProductApp.Data;

namespace ProductApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            using var context = new AppDbContext();
            context.Database.EnsureCreated();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}