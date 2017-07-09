using System;
using System.Windows.Controls;
using System.Windows.Input;
using Plainion.Windows.Controls.Tree;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    partial class DocumentNavigationPane : UserControl
    {
        private DocumentStore myDocumentStore;

        public DocumentNavigationPane()
        {
            Root = new NavigationNode();

            CreateChildCommand = new DelegateCommand<NavigationNode>(OnCreateChild);
            DeleteCommand = new DelegateCommand<NavigationNode>(n => ((NavigationNode)n.Parent).Children.Remove(n));

            var dragDropBehavior = new DragDropBehavior(Root);
            DropCommand = new DelegateCommand<NodeDropRequest>(dragDropBehavior.ApplyDrop);

            InitializeComponent();
        }

        private void OnCreateChild(NavigationNode parent)
        {
            var node = new NavigationNode
            {
                Parent = parent,
            };

            // first add the new node - this will transform the nodes model into a folder if necessary
            parent.Children.Add(node);

            // now we can safely cast to folder
            node.Model = myDocumentStore.Create((Folder)parent.Model, "<new>");

            node.IsSelected = true;
            parent.IsExpanded = true;
        }

        public DocumentStore DocumentStore
        {
            get { return myDocumentStore; }
            set
            {
                myDocumentStore = value;

                Root.Children.Clear();

                AddFolder(Root, myDocumentStore.Root);
            }
        }

        private void AddFolder(NavigationNode node, Folder folder)
        {
            foreach(var child in folder.Children)
            {
                var childNode = new NavigationNode
                {
                    Parent = node,
                    Model = child
                };
                node.Children.Add(childNode);
                AddFolder(childNode, child);
            }
            foreach(var doc in folder.Documents)
            {
                node.Children.Add(new NavigationNode
                {
                    Parent = node,
                    Model = doc
                });
            }
        }

        public NavigationNode Root { get; private set; }

        public ICommand CreateChildCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DropCommand { get; private set; }
    }
}
