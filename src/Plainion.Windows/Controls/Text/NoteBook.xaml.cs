using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.Windows.Controls.Text
{
    public partial class NoteBook : UserControl
    {
        public NoteBook()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            myNavigation.SelectionChanged += OnSelectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // To initialize everything correctly esp. when DocumentStore is null initially
            OnDocumentStoreChanged();
        }

        private void OnSelectionChanged(object sender, IStoreItem item)
        {
            myNotePad.Document = GetFlowDocument(item);

            if (!string.IsNullOrEmpty(myNavigation.SearchText))
            {
                myNotePad.Search(myNavigation.SearchText, SearchMode.All);
            }
        }

        private FlowDocument GetFlowDocument(IStoreItem item)
        {
            if (item == null)
            {
                return null;
            }

            var document = item as Document;
            if (document != null)
            {
                return document.Body;
            }

            return null;
        }

        public static readonly DependencyProperty DocumentStoreProperty = DependencyProperty.Register("DocumentStore",
            typeof(DocumentStore), typeof(NoteBook), new PropertyMetadata(null, OnDocumentStoreChanged));

        private static void OnDocumentStoreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NoteBook)d).OnDocumentStoreChanged();
        }

        private void OnDocumentStoreChanged()
        {
            myNavigation.DocumentStore = DocumentStore;

            if (DocumentStore == null)
            {
                OnSelectionChanged(this, null);
            }
            else
            {
                OnSelectionChanged(this, DocumentStore.Root.Entries.FirstOrDefault());
            }
        }

        public DocumentStore DocumentStore
        {
            get { return (DocumentStore)GetValue(DocumentStoreProperty); }
            set { SetValue(DocumentStoreProperty, value); }
        }

        public static readonly DependencyProperty AutoCorrectionProperty = DependencyProperty.Register("AutoCorrection",
          typeof(AutoCorrectionTable), typeof(NoteBook), new PropertyMetadata(new AutoCorrectionTable(), OnAutoCorrectionChanged));

        private static void OnAutoCorrectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (NoteBook)d;
            self.myNotePad.AutoCorrection = self.AutoCorrection;
        }

        public AutoCorrectionTable AutoCorrection
        {
            get { return (AutoCorrectionTable)GetValue(AutoCorrectionProperty); }
            set { SetValue(AutoCorrectionProperty, value); }
        }
    }
}
