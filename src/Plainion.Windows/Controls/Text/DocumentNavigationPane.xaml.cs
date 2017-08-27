using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
            var document = DocumentStoreExtensions.CreateDocument("<new>");
            var node = myNodeFactory.Create(document, parent);

            parent.Children.Add(node);

            parent.IsExpanded = true;

            node.IsSelected = true;
        }

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = (NavigationNode)sender;
            if (node.IsSelected)
            {
                OnSelectionChanged(node.Model);
            }
        }

        private void OnSelectionChanged(IStoreItem item)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, item);
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

        /// <summary>
        /// Notifies the selected item.
        /// </summary>
        public event EventHandler<IStoreItem> SelectionChanged;

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText",
            typeof(string), typeof(DocumentNavigationPane), new PropertyMetadata(null, OnSearchTextChanged));

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DocumentNavigationPane)d).OnSearchTextChanged();
        }

        private void OnSearchTextChanged()
        {
            SearchResults = DocumentStore.Search(SearchText);
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchResultsProperty = DependencyProperty.Register("SearchResults", typeof(IEnumerable<Document>), typeof(DocumentNavigationPane));

        public IEnumerable<Document> SearchResults
        {
            get { return (IEnumerable<Document>)GetValue(SearchResultsProperty); }
            set { SetValue(SearchResultsProperty, value); }
        }

        private void OnSearchResultSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.OfType<Document>().FirstOrDefault();
            if (item != null)
            {
                OnSelectionChanged(item);
            }
        }
    }
}
