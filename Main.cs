using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KindleImageConverter
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
            myApp.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
            myApp.Run(myWindow);
        }
    }
}
