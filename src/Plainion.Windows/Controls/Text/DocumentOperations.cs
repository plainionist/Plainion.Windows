using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public static class DocumentOperations
    {
        private static Lazy<MethodInfo> GetTextInternal = new Lazy<MethodInfo>(() =>
        {
            var textRangeBase = typeof(TextRange).Assembly.GetType("System.Windows.Documents.TextRangeBase");
            return textRangeBase.GetMethod("GetTextInternal", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TextPointer), typeof(TextPointer) }, null);
        });

        /// <summary>
        /// Returns a TextRange from ContentStart to ContentEnd.
        /// </summary>
        public static TextRange Content(this FlowDocument self)
        {
            Contract.RequiresNotNull(self, "self");

            return new TextRange(self.ContentStart, self.ContentEnd);
        }

        /// <summary>
        /// Returns a text pointer for the given character offset.
        /// </summary>
        public static TextPointer GetPointerFromCharOffset(TextRange range, int charOffset)
        {
            Contract.RequiresNotNull(range, "range");
            Contract.Requires(charOffset >= 0, "charOffset >= 0");

            if (charOffset == 0)
            {
                return range.Start;
            }

            var navigator = range.Start;
            var nextPointer = navigator;
            int counter = 0;
            while (nextPointer != null && counter < charOffset)
            {
                if (nextPointer.CompareTo(range.End) == 0)
                {
                    // If we reach to the end of range, return the EOF pointer.
                    return nextPointer;
                }

                if (nextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    nextPointer = nextPointer.GetNextInsertionPosition(LogicalDirection.Forward);
                    counter++;
                }
                else
                {
                    // If the current pointer is not pointing at a character, we should move to next insertion point 
                    // without incrementing the character counter.
                    nextPointer = nextPointer.GetNextInsertionPosition(LogicalDirection.Forward);
                }
            }

            return nextPointer;
        }

        /// <summary>
        /// Returns only the text of the given range. Solves the problem described here:
        /// https://stackoverflow.com/questions/48240020/wpf-get-textrange-from-listitem-content
        /// ATTENTION: this method uses reflection. In case that fails it returns TextRange.Text.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string TextOnly(this TextRange self)
        {
            Contract.RequiresNotNull(self, "self");

            try
            {
                return (string)GetTextInternal.Value.Invoke(null, new[] { self.Start, self.End });
            }
            catch
            {
                return self.Text;
            }
        }

        /// <summary>
        /// Returns a TextRange covering a word containing or following this TextPointer.
        /// </summary>
        /// <remarks>
        /// If this TextPointer is within a word or at start of word, the containing word range is returned.
        /// If this TextPointer is between two words, the following word range is returned.
        /// If this TextPointer is at trailing word boundary, the preceding word range is returned.
        /// </remarks>
        public static TextRange GetWordAt(TextPointer position)
        {
            Contract.RequiresNotNull(position, "position");

            TextRange wordRange = null;
            TextPointer wordStartPosition = null;

            // Go forward first, to find word end position.
            var wordEndPosition = GetPositionAtWordBoundary(position, LogicalDirection.Forward);

            if (wordEndPosition != null)
            {
                // Then travel backwards, to find word start position.
                wordStartPosition = GetPositionAtWordBoundary(wordEndPosition, LogicalDirection.Backward);
            }

            if (wordStartPosition != null && wordEndPosition != null)
            {
                wordRange = new TextRange(wordStartPosition, wordEndPosition);
            }

            return wordRange;
        }

        /// <summary>
        /// 1.  When wordBreakDirection = Forward, returns a position at the end of the word,
        ///     i.e. a position with a wordBreak character (space) following it.
        /// 2.  When wordBreakDirection = Backward, returns a position at the start of the word,
        ///     i.e. a position with a wordBreak character (space) preceeding it.
        /// 3.  Returns null when there is no workbreak in the requested direction.
        /// </summary>
        private static TextPointer GetPositionAtWordBoundary(TextPointer position, LogicalDirection wordBreakDirection)
        {
            if (!position.IsAtInsertionPosition)
            {
                position = position.GetInsertionPosition(wordBreakDirection);
            }

            var navigator = position;
            while (navigator != null && !IsPositionNextToWordBreak(navigator, wordBreakDirection))
            {
                navigator = navigator.GetNextInsertionPosition(wordBreakDirection);
            }

            return navigator;
        }

        private static bool IsPositionNextToWordBreak(TextPointer position, LogicalDirection wordBreakDirection)
        {
            // Skip over any formatting.
            if (position.GetPointerContext(wordBreakDirection) != TextPointerContext.Text)
            {
                position = position.GetInsertionPosition(wordBreakDirection);
            }

            if (position.GetPointerContext(wordBreakDirection) == TextPointerContext.Text)
            {
                var oppositeDirection = wordBreakDirection == LogicalDirection.Forward ? LogicalDirection.Backward : LogicalDirection.Forward;

                var runBuffer = new char[1];
                var oppositeRunBuffer = new char[1];

                position.GetTextInRun(wordBreakDirection, runBuffer, 0, 1);
                position.GetTextInRun(oppositeDirection, oppositeRunBuffer, 0, 1);

                if ((runBuffer[0] == ' ' || runBuffer[0] == '\t') && oppositeRunBuffer[0] != ' ' && oppositeRunBuffer[0] != '\t')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // We are next to an embedded object or a block boundary
            // -> consider as word break
            return true;
        }

        public static IEnumerable<TextRange> GetWords(TextRange range)
        {
            Contract.RequiresNotNull(range, "range");

            var navigator = range.Start;
            while (navigator != null && navigator.CompareTo(range.End) <= 0)
            {
                var wordRange = GetWordAt(navigator);
                if (wordRange == null || wordRange.IsEmpty)
                {
                    // No more words in the document.
                    yield break;
                }

                yield return wordRange;

                navigator = wordRange.End.GetNextInsertionPosition(LogicalDirection.Forward);
            }
        }

        public static IEnumerable<TextRange> GetLines(TextRange range)
        {
            Contract.RequiresNotNull(range, "range");

            var navigator = range.Start;
            while (navigator != null && navigator.CompareTo(range.End) <= 0)
            {
                var line = GetLineAt(navigator);

                yield return line;

                navigator = line.End.GetNextInsertionPosition(LogicalDirection.Forward);
            }
        }

        public static TextRange GetLineAt(TextPointer pos)
        {
            Contract.RequiresNotNull(pos, "pos");

            var start = FindNewLine(pos, LogicalDirection.Backward);
            var end = FindNewLine(pos, LogicalDirection.Forward);

            var line = new TextRange(start, end);

            if (line.Text.EndsWith(Environment.NewLine))
            {
                // this happens if this is the last line in the document
                line = new TextRange(start, end.GetNextInsertionPosition(LogicalDirection.Backward));
            }

            return line;
        }

        private static TextPointer FindNewLine(TextPointer pos, LogicalDirection direction)
        {
            var range = new TextRange(pos, pos);

            var navigator = pos;
            while (navigator != null && navigator.CompareTo(pos.DocumentStart) >= 0)
            {
                var next = navigator.GetNextInsertionPosition(direction);
                if (next != null)
                {
                    range.Select(next, navigator);
                    if (range.Text == Environment.NewLine)
                    {
                        break;
                    }
                }
                navigator = next;
            }

            if (navigator == null)
            {
                navigator = direction == LogicalDirection.Backward ? pos.DocumentStart : pos.DocumentEnd;
            }

            return navigator;
        }

        // https://stackoverflow.com/questions/1756844/making-a-simple-search-function-making-the-cursor-jump-to-or-highlight-the-wo
        public static IEnumerable<TextRange> Search(TextRange contentRange, TextPointer currentPosition, string searchText, SearchMode mode)
        {
            Contract.RequiresNotNull(contentRange, "contentRange");

            TextRange searchRange;
            var direction = LogicalDirection.Forward;

            if (mode == SearchMode.Next)
            {
                searchRange = new TextRange(currentPosition.GetPositionAtOffset(1), contentRange.End);
            }
            else if (mode == SearchMode.Previous)
            {
                searchRange = new TextRange(contentRange.Start, currentPosition);
                direction = LogicalDirection.Backward;
            }
            else
            {
                searchRange = new TextRange(contentRange.Start, contentRange.End);
            }

            var result = Search(searchRange, searchText, direction);
            if (result != null)
            {
                yield return result;
            }

            if (mode == SearchMode.All)
            {
                while (result != null)
                {
                    result = Search(new TextRange(result.End, contentRange.End), searchText, direction);
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        private static TextRange Search(TextRange searchRange, string searchText, LogicalDirection direction)
        {
            int offset = direction == LogicalDirection.Backward
                ? searchRange.Text.LastIndexOf(searchText, StringComparison.OrdinalIgnoreCase)
                : searchRange.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (offset < 0)
            {
                return null;
            }

            var start = GetPositionAtOffset(searchRange, searchRange.Start, offset, LogicalDirection.Forward);
            var end = GetPositionAtOffset(searchRange, start, searchText.Length, LogicalDirection.Forward);
            return new TextRange(start, end);
        }

        // https://www.codeproject.com/Articles/374721/A-Universal-WPF-Find-Replace-Dialog
        private static TextPointer GetPositionAtOffset(TextRange contentRange, TextPointer startingPoint, int offset, LogicalDirection direction)
        {
            TextPointer binarySearchPoint1 = null;
            TextPointer binarySearchPoint2 = null;

            // setup arguments appropriately
            if (direction == LogicalDirection.Forward)
            {
                binarySearchPoint2 = contentRange.End;

                if (offset < 0)
                {
                    offset = Math.Abs(offset);
                }
            }

            if (direction == LogicalDirection.Backward)
            {
                binarySearchPoint2 = contentRange.Start;

                if (offset > 0)
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

            while (isFound == false)
            {
                if (Math.Abs(offset1) == Math.Abs(offset))
                {
                    isFound = true;
                    resultTextPointer = binarySearchPoint1;
                }
                else if (Math.Abs(offset2) == Math.Abs(offset))
                {
                    isFound = true;
                    resultTextPointer = binarySearchPoint2;
                }
                else
                {
                    if (Math.Abs(offset) < Math.Abs(offset1))
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
                        if (direction == LogicalDirection.Backward)
                        {
                            rtfOffsetMiddle = -rtfOffsetMiddle;
                        }

                        var binarySearchPointMiddle = startingPoint.GetPositionAtOffset(rtfOffsetMiddle, direction);
                        int offsetMiddle = GetOffsetInTextLength(startingPoint, binarySearchPointMiddle);

                        // two cases possible
                        if (Math.Abs(offset) < Math.Abs(offsetMiddle))
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

        private static int GetOffsetInTextLength(TextPointer pointer1, TextPointer pointer2)
        {
            if (pointer1 == null || pointer2 == null)
                return 0;

            TextRange tr = new TextRange(pointer1, pointer2);

            return tr.Text.Length;
        }
    }
}
