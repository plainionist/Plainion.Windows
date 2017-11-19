using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    /// <remarks>
    /// Initial verison inspired by:
    /// http://blogs.msdn.com/b/prajakta/archive/2006/10/17/autp-detecting-hyperlinks-in-richtextbox-part-i.aspx
    /// http://blogs.msdn.com/b/prajakta/archive/2006/11/28/auto-detecting-hyperlinks-in-richtextbox-part-ii.aspx
    /// </remarks>
    class ClickableHyperlink : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            bool ret = false;

            var navigator = range.Start;
            while (navigator != null && navigator.CompareTo(range.End) <= 0)
            {
                var wordRange = DocumentOperations.GetWordRange(navigator);
                if (wordRange == null || wordRange.IsEmpty)
                {
                    // No more words in the document.
                    break;
                }

                string wordText = wordRange.Text;
                var url = TryCreateUrl(wordText);
                if (url != null &&
                    !IsInHyperlinkScope(wordRange.Start) &&
                    !IsInHyperlinkScope(wordRange.End))
                {
                    var hyperlink = new Hyperlink(wordRange.Start, wordRange.End);
                    hyperlink.NavigateUri = url;
                    WeakEventManager<Hyperlink, RequestNavigateEventArgs>.AddHandler(hyperlink, "RequestNavigate", OnHyperlinkRequestNavigate);

                    navigator = hyperlink.ElementEnd.GetNextInsertionPosition(LogicalDirection.Forward);
                    ret = true;
                }
                else
                {
                    navigator = wordRange.End.GetNextInsertionPosition(LogicalDirection.Forward);
                }
            }

            return ret;
        }

        private static bool IsInHyperlinkScope(TextPointer position)
        {
            return GetHyperlinkAncestor(position) != null;
        }

        private static Hyperlink GetHyperlinkAncestor(TextPointer position)
        {
            var parent = position.Parent as Inline;
            while (parent != null && !(parent is Hyperlink))
            {
                parent = parent.Parent as Inline;
            }

            return parent as Hyperlink;
        }

        private static void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private static Uri TryCreateUrl(string wordText)
        {
            if (wordText.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                wordText.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                wordText.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri(wordText);
            }

            if (wordText.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri("http://" + wordText);
            }

            return null;
        }

        public TextPointer TryUndo(TextPointer start)
        {
            var backspacePosition = start.GetNextInsertionPosition(LogicalDirection.Backward);
            Hyperlink hyperlink;
            if (backspacePosition == null || !IsHyperlinkBoundaryCrossed(start, backspacePosition, out hyperlink))
            {
                return null;
            }

            // Remember caretPosition with forward gravity. This is necessary since we are going to delete 
            // the hyperlink element preceeding caretPosition and after deletion current caretPosition 
            // (with backward gravity) will follow content preceeding the hyperlink. 
            // We want to remember content following the hyperlink to set new caret position at.

            var newCaretPosition = start.GetPositionAtOffset(0, LogicalDirection.Forward);

            // Deleting the hyperlink is done using logic below.

            // 1. Copy its children Inline to a temporary array.
            var hyperlinkChildren = hyperlink.Inlines;
            var inlines = new Inline[hyperlinkChildren.Count];
            hyperlinkChildren.CopyTo(inlines, 0);

            // 2. Remove each child from parent hyperlink element and insert it after the hyperlink.
            for (int i = inlines.Length - 1; i >= 0; i--)
            {
                hyperlinkChildren.Remove(inlines[i]);
                hyperlink.SiblingInlines.InsertAfter(hyperlink, inlines[i]);
            }

            // 3. Apply hyperlink's local formatting properties to inlines (which are now outside hyperlink scope).
            var localProperties = hyperlink.GetLocalValueEnumerator();
            var inlineRange = new TextRange(inlines[0].ContentStart, inlines[inlines.Length - 1].ContentEnd);

            while (localProperties.MoveNext())
            {
                var property = localProperties.Current;
                var dp = property.Property;
                object value = property.Value;

                if (!dp.ReadOnly &&
                    dp != Inline.TextDecorationsProperty && // Ignore hyperlink defaults.
                    dp != TextElement.ForegroundProperty &&
                    dp != BaseUriHelper.BaseUriProperty &&
                    !IsHyperlinkProperty(dp))
                {
                    inlineRange.ApplyPropertyValue(dp, value);
                }
            }

            // 4. Delete the (empty) hyperlink element.
            hyperlink.SiblingInlines.Remove(hyperlink);

            return newCaretPosition;
        }

        // Returns true if passed caretPosition and backspacePosition cross a hyperlink end boundary
        // (under the assumption that caretPosition and backSpacePosition are adjacent insertion positions).
        private static bool IsHyperlinkBoundaryCrossed(TextPointer caretPosition, TextPointer backspacePosition, out Hyperlink backspacePositionHyperlink)
        {
            var caretPositionHyperlink = GetHyperlinkAncestor(caretPosition);
            backspacePositionHyperlink = GetHyperlinkAncestor(backspacePosition);

            return (caretPositionHyperlink == null && backspacePositionHyperlink != null) ||
                (caretPositionHyperlink != null && backspacePositionHyperlink != null && caretPositionHyperlink != backspacePositionHyperlink);
        }

        private static bool IsHyperlinkProperty(DependencyProperty dp)
        {
            return dp == Hyperlink.CommandProperty ||
                dp == Hyperlink.CommandParameterProperty ||
                dp == Hyperlink.CommandTargetProperty ||
                dp == Hyperlink.NavigateUriProperty ||
                dp == Hyperlink.TargetNameProperty;
        }
    }
}
