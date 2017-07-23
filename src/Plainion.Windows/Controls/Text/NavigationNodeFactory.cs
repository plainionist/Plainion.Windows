using System;
using System.ComponentModel;

namespace Plainion.Windows.Controls.Text
{
    class NavigationNodeFactory
    {
        private EventHandler<PropertyChangedEventArgs> myOnSelectionChanged;

        public NavigationNodeFactory(EventHandler<PropertyChangedEventArgs> onSelectionChanged)
        {
            myOnSelectionChanged = onSelectionChanged;
        }

        internal NavigationNode Create(IStoreItem model, NavigationNode parent)
        {
            var node = new NavigationNode(this);
            node.Model = model;
            node.Parent = parent;

            PropertyBinding.Observe(() => node.IsSelected, myOnSelectionChanged);

            return node;
        }
    }
}
