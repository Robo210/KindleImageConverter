using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangle_port
{
    class MainApp
    {
        [STAThread] //.net norms for com
        static void Main()
        {
            System.Windows.Application myApp = new System.Windows.Application();
            MainWindow myWindow = new MainWindow();
            MainWindowViewModel viewModel = new MainWindowViewModel();

            myWindow.DataContext = viewModel;
            myWindow.Show();
            myApp.Run();
        }
    }
}
