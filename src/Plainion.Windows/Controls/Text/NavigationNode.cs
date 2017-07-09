using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Plainion.Windows.Controls.Tree;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    class NavigationNode : BindableBase, INode, IDragDropSupport
    {
        private IStoreItem myModel;
        private string myName;
        private bool myIsSelected;
        private bool myIsExpanded;

        public NavigationNode()
        {
            Children = new ObservableCollection<NavigationNode>();
            CollectionChangedEventManager.AddHandler(Children, OnChildrenChanged);
        }

        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var document = myModel as Document;
            if(document != null)
            {

            }

            // TODO: update model
            // also handle that this is a document and needs to get a folder
        }

        public bool IsDragAllowed { get { return true; } }

        public bool IsDropAllowed { get { return true; } }

        public IStoreItem Model
        {
            get { return myModel; }
            set
            {
                myModel = value;
                Name = myModel.Title;
                PropertyBinding.Bind(() => Model.Title, () => Name);
            }
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
