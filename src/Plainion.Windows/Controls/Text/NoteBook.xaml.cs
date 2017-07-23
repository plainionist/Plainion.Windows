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

            myNavigation.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, NavigationNode e)
        {
            var document = e.Model as Document;
            if (document != null)
            {
                myNotePad.Document = document.Body;
            }
            else
            {
                var folder = (Folder)e.Model;
                document = folder.Entries.OfType<Document>().FirstOrDefault();
                if (document != null)
                {
                    myNotePad.Document = document.Body;
                }
                else
                {
                    myNotePad.Document = null;
                }
            }
        }

        public static readonly DependencyProperty DocumentStoreProperty = DependencyProperty.Register("DocumentStore",
            typeof(DocumentStore), typeof(NoteBook), new PropertyMetadata(null, OnDocumentStoreChanged));

        private static void OnDocumentStoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (NoteBook)d;
            self.myNavigation.DocumentStore = self.DocumentStore;
        }

        public DocumentStore DocumentStore
        {
            get { return (DocumentStore)GetValue(DocumentStoreProperty); }
            set { SetValue(DocumentStoreProperty, value); }
        }
    }
}
