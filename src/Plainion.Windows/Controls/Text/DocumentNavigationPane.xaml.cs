using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Plainion.Windows.Controls.Tree;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    partial class DocumentNavigationPane : UserControl
    {
        private NavigationNodeFactory myNodeFactory;
        private DocumentStore myDocumentStore;

        public DocumentNavigationPane()
        {
            myNodeFactory = new NavigationNodeFactory(OnSelectionChanged);
            Root = new NavigationNode(myNodeFactory);

            CreateChildCommand = new DelegateCommand<NavigationNode>(OnCreateChild);
            DeleteCommand = new DelegateCommand<NavigationNode>(n => ((NavigationNode)n.Parent).Children.Remove(n));

            var dragDropBehavior = new DragDropBehavior(Root);
            DropCommand = new DelegateCommand<NodeDropRequest>(dragDropBehavior.ApplyDrop);

            InitializeComponent();
        }

        private void OnCreateChild(NavigationNode parent)
        {
            var document = new Document(() => new FlowDocument()) { Title = "<new>" };
            var node = myNodeFactory.Create(document, parent);

            parent.Children.Add(node);

            node.IsSelected = true;
            parent.IsExpanded = true;
        }

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, (NavigationNode)sender);
            }
        }

        public DocumentStore DocumentStore
        {
            get { return myDocumentStore; }
            set
            {
                myDocumentStore = value;

                using (Root.DisableModelSync())
                {
                    Root.Model = myDocumentStore.Root;
                    Root.Children.Clear();

                    AddFolder(Root, myDocumentStore.Root);
                }
            }
        }

        private void AddFolder(NavigationNode node, Folder folder)
        {
            using (node.DisableModelSync())
            {
                foreach (var child in folder.Entries)
                {
                    var childNode = myNodeFactory.Create(child, node);
                    node.Children.Add(childNode);

                    var childFolder = child as Folder;
                    if (childFolder != null)
                    {
                        AddFolder(childNode, childFolder);
                    }
                }
            }
        }

        public NavigationNode Root { get; private set; }

        public ICommand CreateChildCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DropCommand { get; private set; }

        public event EventHandler<NavigationNode> SelectionChanged;
    }
}
