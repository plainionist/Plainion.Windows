using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Plainion.Windows.Controls.Text
{
    class NavigationNodeFactory
    {
        private EventHandler<PropertyChangedEventArgs> myOnSelectionChanged;

        public NavigationNodeFactory(EventHandler<PropertyChangedEventArgs> onSelectionChanged)
        {
            myOnSelectionChanged = onSelectionChanged;

            if (Dispatcher == null)
            {
                Dispatcher = Application.Current != null ? Application.Current.Dispatcher : null;
            }
        }

        internal NavigationNode Create(IStoreItem model, NavigationNode parent)
        {
            var node = new NavigationNode(this);
            node.Model = model;
            node.Parent = parent;

            PropertyBinding.Observe(() => node.IsSelected, myOnSelectionChanged);

            return node;
        }

        // required for testing
        internal static Dispatcher Dispatcher;
    }
}
