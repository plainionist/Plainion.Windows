using System.Windows;
using System.Windows.Controls;

namespace Plainion.Windows.Controls.Text
{
    public partial class NoteBook : UserControl
    {
        public NoteBook()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DocumentStoreProperty = DependencyProperty.Register("DocumentStore",
            typeof(IDocumentStore), typeof(NotePad), new PropertyMetadata(null, OnDocumentStoreChanged));

        private static void OnDocumentStoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (NoteBook)d;
            //self.myNavigation.Tree = BuildTree(self.DocumentStore);
        }

        public IDocumentStore DocumentStore
        {
            get { return (IDocumentStore)GetValue(DocumentStoreProperty); }
            set { SetValue(DocumentStoreProperty, value); }
        }
    }
}
