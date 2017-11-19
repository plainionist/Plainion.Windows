using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Plainion.Windows.Controls.Text
{
    public partial class NotePad : UserControl
    {
        private bool mySearchTextChangeGuard;

        public NotePad()
        {
            InitializeComponent();

            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), handledEventsToo: false);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Initialize default document from RichTextEditor
            Document = myEditor.Document;
            Document.FontFamily = TextStyles.Body.FontFamily;
            Document.FontSize = TextStyles.Body.FontSize;
        }

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document",
            typeof(FlowDocument), typeof(NotePad), new PropertyMetadata(null, OnDocumentChanged));

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (NotePad)d;
            if(self.Document == null)
            {
                self.myEditor.Document = new FlowDocument();
                self.myEditor.IsReadOnly = true;
            }
            else
            {
                self.myEditor.Document = self.Document;
                self.myEditor.IsReadOnly = false;
            }

            self.myEditor.ClearSearch();
        }

        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = myEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            myBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));

            temp = myEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            myItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));

            temp = myEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            myUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = myEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            myHeadline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(20d));
        }

        private void OnHeadlineClick(object sender, RoutedEventArgs e)
        {
            if(myHeadline.IsChecked == true)
            {
                myEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, TextStyles.Headline.FontSize);
                myEditor.Selection.ApplyPropertyValue(Inline.FontWeightProperty, TextStyles.Headline.FontWeight);
            }
            else
            {
                myEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, TextStyles.Body.FontSize);
                myEditor.Selection.ApplyPropertyValue(Inline.FontWeightProperty, TextStyles.Body.FontWeight);
            }
        }

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText",
            typeof(string), typeof(NotePad), new PropertyMetadata(null, OnSearchTextChanged));

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NotePad)d).OnSearchTextChanged();
        }

        private void OnSearchTextChanged()
        {
            if (!mySearchTextChangeGuard)
            {
                Search(SearchText, SearchMode.Initial);
            }
        }

        public void Search(string text, SearchMode mode)
        {
            if(string.IsNullOrEmpty(text))
            {
                myEditor.ClearSearch();

                SearchSuccessful = true;
            }
            else
            {
                mySearchTextChangeGuard = true;

                SearchText = text;
                SearchSuccessful = myEditor.Search(text, mode);

                mySearchTextChangeGuard = false;
            }
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchSuccessfulProperty = DependencyProperty.Register("SearchSuccessful", typeof(bool), typeof(NotePad), new PropertyMetadata(true));

        public bool SearchSuccessful
        {
            get { return (bool)GetValue(SearchSuccessfulProperty); }
            set { SetValue(SearchSuccessfulProperty, value); }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F3)
            {
                if(Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    myEditor.Search(SearchText, SearchMode.Previous);
                }
                else
                {
                    myEditor.Search(SearchText, SearchMode.Next);
                }
            }
            else if(e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                mySearchBox.Focus();
                mySearchBox.SelectAll();
            }
        }
    }
}
