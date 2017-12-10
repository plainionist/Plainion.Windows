using System;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkdownHeadline : IAutoCorrection
    {
        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            Headline last = null;

            foreach (var line in DocumentOperations.GetLines(input.Range))
            {
                if (line.Text.StartsWith("# "))
                {
                    last = InsertHeadline(line, 0);
                }
                else if (line.Text.StartsWith("## "))
                {
                    last = InsertHeadline(line, 1);
                }
                else if (line.Text.StartsWith("### "))
                {
                    last = InsertHeadline(line, 2);
                }
            }

            // TODO: only on "ENTER" --> we need an input container and a result container for the caret pos
            // add a new body Run if user hit enter after existing headline
            if (last == null && Headline.IsHeadline(input.Range.End))
            {
                var body = new Body(string.Empty, input.Range.End);
                RichTextEditor.ME.CaretPosition = body.ContentStart;
            }

            return new AutoCorrectionResult(last != null);
        }

        private Headline InsertHeadline(TextRange line, int level)
        {
            var text = line.Text.Substring(level + 1).Trim();
            line.Text = string.Empty;

            var headline = new Headline(text, line.Start, TextStyles.Headlines[level]);

            if (string.IsNullOrEmpty(text))
            {
                RichTextEditor.ME.CaretPosition = headline.ContentStart;
            }

            return headline;
        }

        public AutoCorrectionResult TryUndo(TextPointer start)
        {
            return new AutoCorrectionResult(false);
        }
    }
}
