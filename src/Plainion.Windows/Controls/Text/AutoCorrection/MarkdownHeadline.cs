using System;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkdownHeadline : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            bool ret = false;

            var lineStart = new Lazy<TextPointer>(() => DocumentOperations.GetLineAt(range.Start).Start);

            foreach (var wordRange in DocumentOperations.GetWords(range))
            {
                if (wordRange.Start.CompareTo(lineStart.Value) == 0 && wordRange.Text == "##")
                {
                    wordRange.Text = string.Empty;

                    var run = new Run(string.Empty, wordRange.Start);
                    run.FontSize = TextStyles.Headline.FontSize;
                    run.FontWeight = TextStyles.Headline.FontWeight;

                    ret = true;
                }
            }

            return ret;
        }

        public bool TryUndo(TextPointer start)
        {
            return false;
        }
    }
}
