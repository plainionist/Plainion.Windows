using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Plainion.Windows.Controls.Text
{
    public partial class NoteBook : UserControl
    {
        public NoteBook()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DocumentStoreProperty = DependencyProperty.Register("DocumentStore",
            typeof(DocumentStore), typeof(NoteBook), new PropertyMetadata(null, OnDocumentStoreChanged));

        private static void OnDocumentStoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (NoteBook)d;

            var root = self.myNavigation.Root;

            root.Children.Clear();

            AddFolder(root, self.DocumentStore.Root);
        }

        private static void AddFolder(NavigationNode node, Folder folder)
        {
            //foreach (var child in folder.Children)
            //{
            //    var childNode = new NavigationNode
            //    {
            //        Parent = node,
            //        Folder = folder
            //    };
            //    node.Children.Add(childNode);
            //    AddFolder(childNode, child);
            //}
            //foreach (var doc in folder.Documents)
            //{
            //    node.Children.Add(new NavigationNode
            //    {
            //        Parent = node,
            //        Document = doc
            //    });
            //}
        }

        public DocumentStore DocumentStore
        {
            get { return (DocumentStore)GetValue(DocumentStoreProperty); }
            set { SetValue(DocumentStoreProperty, value); }
        }
    }
}
