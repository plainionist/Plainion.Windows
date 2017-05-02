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

            foreach(var process in Process.GetProcesses())
            {
                var processNode = new NavigationNode
                {
                    Parent = root,
                    Id = process.Id.ToString(),
                    Name = process.ProcessName,
                    IsDragAllowed = false
                };
                root.Children.Add(processNode);

                var children = process.Threads
                    .OfType<ProcessThread>()
                    .Select(t => new NavigationNode
                    {
                        Parent = processNode,
                        Id = t.Id.ToString(),
                        Name = "unknown",
                        IsDropAllowed = false
                    });
                foreach(var child in children)
                {
                    processNode.Children.Add(child);
                }
            }
        }

        public IDocumentStore DocumentStore
        {
            get { return (IDocumentStore)GetValue(DocumentStoreProperty); }
            set { SetValue(DocumentStoreProperty, value); }
        }
    }
}
