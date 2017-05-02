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
            typeof(IDocumentStore), typeof(NoteBook), new PropertyMetadata(null, OnDocumentStoreChanged));

        private static void OnDocumentStoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (NoteBook)d;

            var root = self.myNavigation.Root;

            root.Children.Clear();

            foreach(var doc in self.DocumentStore.All)
            {
                var node = new NavigationNode
                {
                    Parent = root,
                    Name = doc.Path.AsPath
                };
                root.Children.Add(node);

                //var children = doc.Threads
                //    .OfType<ProcessThread>()
                //    .Select(t => new NavigationNode
                //    {
                //        Parent = processNode,
                //        Name = "unknown"
                //    });
                //foreach(var child in children)
                //{
                //    processNode.Children.Add(child);
                //}
            }
        }

        public IDocumentStore DocumentStore
        {
            get { return (IDocumentStore)GetValue(DocumentStoreProperty); }
            set { SetValue(DocumentStoreProperty, value); }
        }
    }
}
