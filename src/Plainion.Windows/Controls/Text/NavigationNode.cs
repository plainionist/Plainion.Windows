using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plainion.Windows.Controls.Tree;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    class NavigationNode : BindableBase, INode, IDragDropSupport
    {
        private string myName;
        private bool myIsSelected;
        private bool myIsExpanded;

        public NavigationNode()
        {
            Children = new ObservableCollection<NavigationNode>();
        }

        public bool IsDragAllowed { get { return true; } }

        public bool IsDropAllowed { get { return true; } }

        public Document Document { get; set; }

        public string Name
        {
            get { return myName; }
            set
            {
                if(SetProperty(ref myName, value))
                {
                    // TODO: ?!? :(
                    //Document.Path.Name = myName;
                }
            }
        }

        IEnumerable<INode> INode.Children
        {
            get { return Children; }
        }

        // TODO: we need to sync to model
        public ObservableCollection<NavigationNode> Children { get; private set; }

        // TODO: we need to sync to model
        public INode Parent { get; set; }

        public bool IsSelected
        {
            get { return myIsSelected; }
            set { SetProperty(ref myIsSelected, value); }
        }

        public bool IsExpanded
        {
            get { return myIsExpanded; }
            set { SetProperty(ref myIsExpanded, value); }
        }

        bool INode.Matches(string pattern)
        {
            if(pattern == "*")
            {
                return Name != null;
            }

            return Name != null && Name.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}
