using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            TextRange searchRange;

            if(mode == SearchMode.Next)
            {
                searchRange = new TextRange(Selection.Start.GetPositionAtOffset(1), Document.ContentEnd);
            }
            else if(mode == SearchMode.Previous)
            {
                searchRange = new TextRange(Document.ContentStart, Selection.Start);
            }
            else
            {
                searchRange = new TextRange(Document.ContentStart, Document.ContentEnd);
            }

            ClearSearch();

            var foundRange = FindAndHighlight(searchRange, searchText, mode);
            if(foundRange == null)
            {
                return false;
            }

            if(mode == SearchMode.All)
            {
                while(foundRange != null)
                {
                    foundRange = FindAndHighlight(new TextRange(foundRange.End, Document.ContentEnd), searchText, mode);
                }
            }

            return true;
        }

        private TextRange FindAndHighlight(TextRange searchRange, string searchText, SearchMode mode)
        {
            var foundRange = FindTextInRange(searchRange, searchText, mode);
            if(foundRange == null)
            {
                return null;
            }

            Selection.Select(foundRange.Start, foundRange.End);
            Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);

            return foundRange;
        }

        private TextRange FindTextInRange(TextRange searchRange, string searchText, SearchMode mode)
        {
            int offset = mode == SearchMode.Previous
                ? searchRange.Text.LastIndexOf(searchText, StringComparison.OrdinalIgnoreCase)
                : searchRange.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if(offset < 0)
            {
                return null;
            }

            var start = GetPositionAtOffset(searchRange.Start, offset, LogicalDirection.Forward);
            var end = GetPositionAtOffset(start, searchText.Length, LogicalDirection.Forward);
            return new TextRange(start, end);
        }

        // https://www.codeproject.com/Articles/374721/A-Universal-WPF-Find-Replace-Dialog
        private TextPointer GetPositionAtOffset(TextPointer startingPoint, int offset, LogicalDirection direction)
        {
            TextPointer binarySearchPoint1 = null;
            TextPointer binarySearchPoint2 = null;

            // setup arguments appropriately
            if(direction == LogicalDirection.Forward)
            {
                binarySearchPoint2 = Document.ContentEnd;

                if(offset < 0)
                {
                    offset = Math.Abs(offset);
                }
            }

            if(direction == LogicalDirection.Backward)
            {
                binarySearchPoint2 = Document.ContentStart;

                if(offset > 0)
                {
                    offset = -offset;
                }
            }

            // setup for binary search
            bool isFound = false;
            TextPointer resultTextPointer = null;

            int offset2 = Math.Abs(GetOffsetInTextLength(startingPoint, binarySearchPoint2));
            int halfOffset = direction == LogicalDirection.Backward ? -(offset2 / 2) : offset2 / 2;

            binarySearchPoint1 = startingPoint.GetPositionAtOffset(halfOffset, direction);
            int offset1 = Math.Abs(GetOffsetInTextLength(startingPoint, binarySearchPoint1));

            // binary search loop

            while(isFound == false)
            {
                if(Math.Abs(offset1) == Math.Abs(offset))
                {
                    isFound = true;
                    resultTextPointer = binarySearchPoint1;
                }
                else
                    if(Math.Abs(offset2) == Math.Abs(offset))
                    {
                        isFound = true;
                        resultTextPointer = binarySearchPoint2;
                    }
                    else
                    {
                        if(Math.Abs(offset) < Math.Abs(offset1))
                        {
                            // this is simple case when we search in the 1st half
                            binarySearchPoint2 = binarySearchPoint1;
                            offset2 = offset1;

                            halfOffset = direction == LogicalDirection.Backward ? -(offset2 / 2) : offset2 / 2;

                            binarySearchPoint1 = startingPoint.GetPositionAtOffset(halfOffset, direction);
                            offset1 = Math.Abs(GetOffsetInTextLength(startingPoint, binarySearchPoint1));
                        }
                        else
                        {
                            // this is more complex case when we search in the 2nd half
                            int rtfOffset1 = startingPoint.GetOffsetToPosition(binarySearchPoint1);
                            int rtfOffset2 = startingPoint.GetOffsetToPosition(binarySearchPoint2);
                            int rtfOffsetMiddle = (Math.Abs(rtfOffset1) + Math.Abs(rtfOffset2)) / 2;
                            if(direction == LogicalDirection.Backward)
                            {
                                rtfOffsetMiddle = -rtfOffsetMiddle;
                            }

                            TextPointer binarySearchPointMiddle = startingPoint.GetPositionAtOffset(rtfOffsetMiddle, direction);
                            int offsetMiddle = GetOffsetInTextLength(startingPoint, binarySearchPointMiddle);

                            // two cases possible
                            if(Math.Abs(offset) < Math.Abs(offsetMiddle))
                            {
                                // 3rd quarter of search domain
                                binarySearchPoint2 = binarySearchPointMiddle;
                                offset2 = offsetMiddle;
                            }
                            else
                            {
                                // 4th quarter of the search domain
                                binarySearchPoint1 = binarySearchPointMiddle;
                                offset1 = offsetMiddle;
                            }
                        }
                    }
            }

            return resultTextPointer;
        }

        /// <summary>
        /// returns length of a text between two text pointers
        /// </summary>
        /// <param name="pointer1"></param>
        /// <param name="pointer2"></param>
        /// <returns></returns>
        int GetOffsetInTextLength(TextPointer pointer1, TextPointer pointer2)
        {
            if(pointer1 == null || pointer2 == null)
                return 0;

            TextRange tr = new TextRange(pointer1, pointer2);

            return tr.Text.Length;
        }

        public void ClearSearch()
        {
            new TextRange(Document.ContentStart, Document.ContentEnd)
                .ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
        }
    }
}
