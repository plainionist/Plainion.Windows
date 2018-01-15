using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    /// <summary>
    /// Converts URLs into clickable hyperlinks
    /// </summary>
    /// <remarks>
    /// Initial verison inspired by:
    /// http://blogs.msdn.com/b/prajakta/archive/2006/10/17/autp-detecting-hyperlinks-in-richtextbox-part-i.aspx
    /// http://blogs.msdn.com/b/prajakta/archive/2006/11/28/auto-detecting-hyperlinks-in-richtextbox-part-ii.aspx
    /// </remarks>
    public class ClickableHyperlink : IAutoCorrection
    {
        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            bool success = false;

            foreach (var wordRange in DocumentOperations.GetWords(input.Range))
            {
                string wordText = wordRange.TextOnly();
                var url = TryCreateUrl(wordText);
                if (url != null && !IsInHyperlinkScope(wordRange.Start) && !IsInHyperlinkScope(wordRange.End))
                {
                    var hyperlink = new Hyperlink(wordRange.Start, wordRange.End);
                    hyperlink.NavigateUri = url;
                    WeakEventManager<Hyperlink, RequestNavigateEventArgs>.AddHandler(hyperlink, "RequestNavigate", OnHyperlinkRequestNavigate);

                    success = true;
                }
            }

            return new AutoCorrectionResult(success);
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
            try
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
            catch
            {
                return null;
            }
        }

        public AutoCorrectionResult TryUndo(TextPointer start)
        {
            var hyperlink = start.Parent as Hyperlink;
            if (hyperlink == null)
            {
                var para = start.Parent as Paragraph;
                if (para != null)
                {
                    hyperlink = para.Inlines.OfType<Hyperlink>().SingleOrDefault();
                }

                if (hyperlink == null)
                {
                    return new AutoCorrectionResult(false);
                }
            }

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

            return new AutoCorrectionResult(true);
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
