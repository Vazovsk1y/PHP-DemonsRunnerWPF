using System;

namespace DemonsRunner
{
    internal class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
