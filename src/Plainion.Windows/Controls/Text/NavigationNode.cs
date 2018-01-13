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
        private NavigationNodeFactory myFactory;
        private IStoreItem myModel;
        private string myName;
        private bool myIsSelected;
        private bool myIsExpanded;
        private BindingId myModelBindingId;
        private int myChildObservationCount;

        public NavigationNode(NavigationNodeFactory factory)
        {
            myFactory = factory;

            Children = new ObservableCollection<NavigationNode>();
            RegisterChangeHandler();
        }

        private void RegisterChangeHandler()
        {
            if (myChildObservationCount == 0)
            {
                CollectionChangedEventManager.AddHandler(Children, OnChildrenChanged);
            }

            myChildObservationCount++;
        }

        private void UnregisterChangeHandler()
        {
            if (myChildObservationCount == 1)
            {
                CollectionChangedEventManager.RemoveHandler(Children, OnChildrenChanged);
            }
            myChildObservationCount--;
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
                var folder = (Folder)myModel;
                folder.MoveEntry(e.OldStartingIndex, e.NewStartingIndex);
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
                folder = ConvertToFolder((Document)Model);
            }

            foreach (var item in items)
            {
                Contract.RequiresNotNull(item.Model != null, "item.Model");
                folder.Entries.Insert(startIndex, item.Model);
                startIndex++;
            }
        }

        private Folder ConvertToFolder(Document model)
        {
            // convert model to folder
            var folder = new Folder();
            folder.Title = myModel.Title;

            var parent = (Folder)((NavigationNode)Parent).Model;
            parent.Entries.Insert(parent.Entries.IndexOf(model), folder);
            parent.Entries.Remove(model);

            if (!string.IsNullOrWhiteSpace(model.Body.Content().Text))
            {
                // if the "old" document has content ad it as first child.
                // we cannot modify the children collection from CollectionChanged event 
                NavigationNodeFactory.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ((Folder)Model).Entries.Add(model);

                    var child = myFactory.Create(model, this);

                    Children.Insert(0, child);
                }));
            }

            // last update my model reference
            Model = folder;

            return folder;
        }

        private void RemoveOldItems(IEnumerable<NavigationNode> items)
        {
            var folder = (Folder)Model;
            foreach (var item in items)
            {
                folder.Entries.Remove(item.Model);
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
                if (myModel != null)
                {
                    PropertyBinding.Unbind(myModelBindingId);
                }

                myModel = value;

                if (myModel != null)
                {
                    Name = myModel.Title;

                    myModelBindingId = PropertyBinding.Bind(() => Model.Title, () => Name);
                }
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
