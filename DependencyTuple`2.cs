// (c) Kyle Sabo 2011

using System.Windows;

namespace mangle_port
{
    public class DependencyTuple<T1, T2> : DependencyObject
    {
        // Item1 property
        public static readonly DependencyProperty Item1Property = DependencyProperty.Register(
            "Item1",
            typeof(T1),
            typeof(DependencyTuple<T1, T2>));

        public T1 Item1
        {
            get
            {
                return (T1)GetValue(Item1Property);
            }
            set
            {
                SetValue(Item1Property, value);
            }
        }

        // Item2 property
        public static readonly DependencyProperty Item2Property = DependencyProperty.Register(
            "Item2",
            typeof(T2),
            typeof(DependencyTuple<T1, T2>));

        public T2 Item2
        {
            get
            {
                return (T2)GetValue(Item1Property);
            }
            set
            {
                SetValue(Item2Property, value);
            }
        }

        public DependencyTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
