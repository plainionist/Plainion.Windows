using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Plainion.Windows.Controls.Text
{
    /// <summary>
    /// Extends RichtTextBox by auto-completion.
    /// Current implementation supports: 
    /// - autodetection of hyperlinks
    /// </summary>
    /// <remarks>
    /// Initial verison inspired by:
    /// http://blogs.msdn.com/b/prajakta/archive/2006/10/17/autp-detecting-hyperlinks-in-richtextbox-part-i.aspx
    /// http://blogs.msdn.com/b/prajakta/archive/2006/11/28/auto-detecting-hyperlinks-in-richtextbox-part-ii.aspx
    /// </remarks>
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
            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), handledEventsToo: true);

            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPasted);

            // required to get hyperlinks working
            IsDocumentEnabled = true;
        }

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
                // Hyperlink detection will be done in OnTextChanged()
            }
            else // Key.Back
            {
                var newCaretPosition = DocumentFacade.RemoveHyperlink(Selection.Start);
                if(newCaretPosition != null)
                {
                    // Update selection, since we deleted Hyperlink element and caretPosition was at that Hyperlink's end boundary.
                    Selection.Select(newCaretPosition, newCaretPosition);
                }
            }
        }

        private void OnPasted(object sender, DataObjectPastingEventArgs e)
        {
            myWordsAdded = true;
            mySelectionStartPosition = Selection.Start;
            mySelectionEndPosition = Selection.IsEmpty ? Selection.End.GetPositionAtOffset(0, LogicalDirection.Forward) : Selection.End;

            // Hyperlink detection will be done in OnTextChanged()
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if(!myWordsAdded || Document == null)
            {
                return;
            }

            TextChanged -= OnTextChanged;

            DocumentFacade.TryMakeHyperlinks(new TextRange(mySelectionStartPosition, mySelectionEndPosition));

            TextChanged += OnTextChanged;

            myWordsAdded = false;
            mySelectionStartPosition = null;
            mySelectionEndPosition = null;
        }

        // https://stackoverflow.com/questions/1756844/making-a-simple-search-function-making-the-cursor-jump-to-or-highlight-the-wo
        public bool Search(string searchText, SearchMode mode)
        {
            if(Document == null)
            {
                return false;
            }

            ClearSearch();

            var results = DocumentFacade.Search(Document, Selection.Start, searchText, mode).ToList();

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
