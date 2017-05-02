using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Plainion.Windows.Controls.Tree;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    class NavigationNode : BindableBase, INode, IDragDropSupport
    {
        private string myId;
        private string myName;
        private bool myIsSelected;
        private bool myIsExpanded;
        private bool? myIsChecked;

        public NavigationNode()
        {
            Children = new ObservableCollection<NavigationNode>();

            IsDragAllowed = true;
            IsDropAllowed = true;
            IsChecked = false;
        }

        public bool IsDragAllowed { get; set; }

        public bool IsDropAllowed { get; set; }

        public string Id
        {
            get { return myId; }
            set { SetProperty(ref myId, value); }
        }

        public string Name
        {
            get { return myName; }
            set { SetProperty(ref myName, value); }
        }

        IEnumerable<INode> INode.Children
        {
            get { return Children; }
        }

        public ObservableCollection<NavigationNode> Children { get; private set; }

        public INode Parent { get; set; }

        public bool? IsChecked
        {
            get { return myIsChecked; }
            set { SetProperty(ref myIsChecked, value); }
        }

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

            return (Name != null && Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                || (Id != null && Id.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
    }
}
