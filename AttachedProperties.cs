﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace mangle_port
{
    public class AttachedProperties
    {
        public static DependencyProperty RegisterCommandBindingsProperty = DependencyProperty.RegisterAttached(
            "RegisterCommandBindings",
            typeof(CommandBindingCollection),
            typeof(AttachedProperties),
            new PropertyMetadata(null, OnRegisterCommandBindingChanged));

        public static void SetRegisterCommandBindings(UIElement element, CommandBindingCollection value)
        {
            if (element != null)
                element.SetValue(RegisterCommandBindingsProperty, value);
        }
        public static CommandBindingCollection GetRegisterCommandBindings(UIElement element)
        {
            return (element != null ? (CommandBindingCollection)element.GetValue(RegisterCommandBindingsProperty) : null);
        }
        private static void OnRegisterCommandBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element != null)
            {
                CommandBindingCollection bindings = e.NewValue as CommandBindingCollection;
                if (bindings != null)
                {
                    // clear the collection first
                    element.CommandBindings.Clear();
                    element.CommandBindings.AddRange(bindings);
                }
            }
        }
    }
}
