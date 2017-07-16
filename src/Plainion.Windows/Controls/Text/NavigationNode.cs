using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
        }

        private void RegisterChangeHandler()
        {
            CollectionChangedEventManager.AddHandler(Children, OnChildrenChanged);
        }

        private void UnregisterChangeHandler()
        {
            CollectionChangedEventManager.RemoveHandler(Children, OnChildrenChanged);
        }

        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Contract.Invariant(e.OldItems == null, "Existence of OldItems not expected");
                AddNewItems(e.NewStartingIndex, e.NewItems.Cast<NavigationNode>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                throw new NotImplementedException(e.Action.ToString());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                Contract.Invariant(e.NewItems == null, "Existence of NewItems not expected");
                RemoveOldItems(e.OldItems.Cast<NavigationNode>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                throw new NotImplementedException(e.Action.ToString());
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (e.NewItems != null)
                {
                    AddNewItems(e.NewStartingIndex, e.NewItems.Cast<NavigationNode>());
                }
                if (e.OldItems != null)
                {
                    RemoveOldItems(e.OldItems.Cast<NavigationNode>());
                }
            }
            else
            {
                throw new NotSupportedException("Unknown action: " + e.Action);
            }
        }

        private void AddNewItems(int startIndex, IEnumerable<NavigationNode> items)
        {
            var folder = myModel as Folder;
            if (folder == null)
            {
                // convert to folder
                folder = new Folder();
                folder.Title = myModel.Title;

                var child = new NavigationNode();
                child.Model = Model;
                child.Parent = this;

                Model = folder;
                Children.Add(child);
            }

            foreach (var item in items)
            {
                var childFolder = item.Model as Folder;
                if (childFolder != null)
                {
                    folder.Children.Insert(startIndex, childFolder);
                }
                else
                {
                    folder.Documents.Insert(startIndex, (Document)item.Model);
                }
                startIndex++;
            }
        }

        private void RemoveOldItems(IEnumerable<NavigationNode> items)
        {
            var folder = (Folder)Model;
            foreach (var item in items)
            {
                var childFolder = item.Model as Folder;
                if (childFolder != null)
                {
                    folder.Children.Remove(childFolder);
                }
                else
                {
                    folder.Documents.Remove((Document)item.Model);
                }
            }
        }

        private class ModelUpdateGuard : IDisposable
        {
            private NavigationNode myOwner;

            public ModelUpdateGuard(NavigationNode owner)
            {
                myOwner = owner;
                myOwner.UnregisterChangeHandler();
            }

            public void Dispose()
            {
                myOwner.RegisterChangeHandler();
                myOwner = null;
            }
        }

        public IDisposable DisableModelSync()
        {
            return new ModelUpdateGuard(this);
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
            if (pattern == "*")
            {
                return Name != null;
            }

            return Name != null && Name.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}
