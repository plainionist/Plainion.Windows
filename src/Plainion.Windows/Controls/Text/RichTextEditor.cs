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

        // True when word(s) are added to this RichTextBox.
        private bool myWordsAdded;

        // TextPointers that track the range covering content where words are added.
        private TextPointer mySelectionStartPosition;
        private TextPointer mySelectionEndPosition;

        public RichTextEditor()
        {
            AutoCorrection = new AutoCorrectionTable();

            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), handledEventsToo: true);

            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPasted);

            // required to get hyperlinks working
            IsDocumentEnabled = true;
        }

        public AutoCorrectionTable AutoCorrection { get; private set; }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Back && e.Key != Key.Space && e.Key != Key.Return)
            {
                return;
            }

            if(!Selection.IsEmpty)
            {
                Selection.Text = string.Empty;
            }

            if(e.Key == Key.Space || e.Key == Key.Return)
            {
                myWordsAdded = true;
                mySelectionStartPosition = Selection.Start;
                mySelectionEndPosition = Selection.End.GetPositionAtOffset(0, LogicalDirection.Forward);
                
                // auto correction detection will be done in OnTextChanged()
            }
            else // Key.Back
            {
                var newCaretPosition = AutoCorrection.Undo(Selection.Start);
                if(newCaretPosition != null)
                {
                    // Update selection, since we deleted a auto correction element and caretPosition was at that auto correction's end boundary.
                    Selection.Select(newCaretPosition, newCaretPosition);
                }
            }
        }

        private void OnPasted(object sender, DataObjectPastingEventArgs e)
        {
            myWordsAdded = true;
            mySelectionStartPosition = Selection.Start;
            mySelectionEndPosition = Selection.IsEmpty ? Selection.End.GetPositionAtOffset(0, LogicalDirection.Forward) : Selection.End;

            // auto correction detection will be done in OnTextChanged()
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if(!myWordsAdded || Document == null)
            {
                return;
            }

            TextChanged -= OnTextChanged;

            AutoCorrection.Apply(new TextRange(mySelectionStartPosition, mySelectionEndPosition));

            TextChanged += OnTextChanged;

            myWordsAdded = false;
            mySelectionStartPosition = null;
            mySelectionEndPosition = null;
        }

        public bool Search(string searchText, SearchMode mode)
        {
            if(Document == null)
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
