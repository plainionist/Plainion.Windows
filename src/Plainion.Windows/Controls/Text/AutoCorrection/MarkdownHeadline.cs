using System;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    internal class Headline : Run
    {
        public Headline()
            : this(string.Empty, null, TextStyles.Headlines[1])
        {
        }

        public Headline(string text, TextPointer insertionPosition, TextStyles.Headline headline)
            : base(text, insertionPosition)
        {
            FontFamily = headline.FontFamily;
            FontSize = headline.FontSize;
            FontWeight = headline.FontWeight;
        }
    }

    public class MarkdownHeadline : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            bool success = false;

            foreach (var line in DocumentOperations.GetLines(range))
            {
                if (line.Text.StartsWith("# "))
                {
                    InsertHeadline(line, 0);
                    success = true;
                }
                else if (line.Text.StartsWith("## "))
                {
                    InsertHeadline(line, 1);
                    success = true;
                }
                else if (line.Text.StartsWith("### "))
                {
                    InsertHeadline(line, 2);
                    success = true;
                }
            }

            if (success && range.End.CompareTo(range.End.DocumentEnd) <= 0)
            {
                // if we converted the last line in the document
                // -> add a new one in body text style
                new Run(Environment.NewLine, range.End);
            }

            return success;
        }

        private void InsertHeadline(TextRange line, int level)
        {
            var text = line.Text.Substring(level + 1).Trim();
            line.Text = string.Empty;

            new Headline(text, line.Start, TextStyles.Headlines[level]);
        }

        public bool TryUndo(TextPointer start)
        {
            return false;
        }
    }
}
