// (c) Kyle Sabo 2011

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace mangle_port
{
    public class MainWindowCommands
    {
        public static RoutedCommand AddFiles = new RoutedCommand();
        public static RoutedCommand RemoveFiles = new RoutedCommand();
        public static RoutedCommand AddDirectory = new RoutedCommand();
        public static RoutedCommand ExportFiles = new RoutedCommand();
    }
}
