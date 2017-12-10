using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.Windows.Controls.Text
{
    /// <summary>
    /// Extends RichtTextBox by auto-completion.
    /// See "AutoCorrection" namespace for more details
    /// </summary>
    public class RichTextEditor : RichTextBox
    {
        internal static Brush SearchHighlightBrush = Brushes.Yellow;

        private AutoCorrectionTrigger? myAutoCorrectionTrigger;

        // TextPointers that track the range covering content where words are added.
        private int mySelectionStartPosition;

        public RichTextEditor()
        {
            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), handledEventsToo: true);

            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPasted);

            // required to get hyperlinks working
            IsDocumentEnabled = true;
        }

        public static readonly DependencyProperty AutoCorrectionProperty = DependencyProperty.Register("AutoCorrection",
            typeof(AutoCorrectionTable), typeof(RichTextEditor), new PropertyMetadata(new AutoCorrectionTable()));

        public AutoCorrectionTable AutoCorrection
        {
            get { return (AutoCorrectionTable)GetValue(AutoCorrectionProperty); }
            set { SetValue(AutoCorrectionProperty, value); }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Back && e.Key != Key.Space && e.Key != Key.Return)
            {
                return;
            }

            if (!Selection.IsEmpty)
            {
                Selection.Text = string.Empty;
            }

            if (e.Key == Key.Space || e.Key == Key.Return)
            {
                myAutoCorrectionTrigger = e.Key == Key.Space ? AutoCorrectionTrigger.Space : AutoCorrectionTrigger.Return;

                // we get "next insertion position backwards" to "skip" the space or enter and want to remember "last content" typed
                mySelectionStartPosition = Document.ContentStart.GetOffsetToPosition(Selection.Start.GetNextInsertionPosition(LogicalDirection.Backward));

                // auto correction detection will be done in OnTextChanged()

                if (e.Key == Key.Return)
                {
                    // we have to trigger auto correction explicitly here
                    ApplyAutoCorrection();
                }
            }
            else // Key.Back
            {
                // Remember caretPosition with forward gravity. This is necessary since we might remove highlighting elements preceeding caretPosition 
                // and after deletion current caretPosition (with backward gravity) will follow content preceeding the highlighting. 
                // We want to remember content following the highlighting to set new caret position at.

                var newCaretPosition = Selection.Start.GetPositionAtOffset(0, LogicalDirection.Forward);

                if (AutoCorrection.Undo(Selection.Start).Success)
                {
                    // Update selection, since we deleted a auto correction element and caretPosition was at that auto correction's end boundary.
                    Selection.Select(newCaretPosition, newCaretPosition);
                }
            }
        }

        private void OnPasted(object sender, DataObjectPastingEventArgs e)
        {
            myAutoCorrectionTrigger = AutoCorrectionTrigger.CopyAndPaste;
            mySelectionStartPosition = Document.ContentStart.GetOffsetToPosition(CaretPosition);

            // auto correction detection will be done in OnTextChanged()
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyAutoCorrection();
        }

        private void ApplyAutoCorrection()
        {
            if (myAutoCorrectionTrigger == null || Document == null)
            {
                return;
            }

            TextChanged -= OnTextChanged;

            var intput = new AutoCorrectionInput(new TextRange(Document.ContentStart.GetPositionAtOffset(mySelectionStartPosition), CaretPosition), myAutoCorrectionTrigger.Value);
            var result = AutoCorrection.Apply(intput);

            if (result.Success && result.CaretPosition != null)
            {
                CaretPosition = result.CaretPosition;
            }

            TextChanged += OnTextChanged;

            myAutoCorrectionTrigger = null;
        }

        public bool Search(string searchText, SearchMode mode)
        {
            if (Document == null)
            {
                return false;
            }

            ClearSearch();

            var results = DocumentOperations.Search(Document, Selection.Start, searchText, mode).ToList();

            foreach (var result in results)
            {
                Selection.Select(result.Start, result.End);
                Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SearchHighlightBrush);
            }

            return results.Count > 0;
        }

        public void ClearSearch()
        {
            new TextRange(Document.ContentStart, Document.ContentEnd)
                .ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
        }
    }
}
