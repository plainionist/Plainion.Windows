using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkdownHeadline : IAutoCorrection
    {
        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            // add a new body Run if user hit enter after existing headline
            if (input.Trigger == AutoCorrectionTrigger.Return && input.Range.End.Parent is Headline)
            {
                var body = new Body(string.Empty, input.Range.End);
                return new AutoCorrectionResult(true, body.ContentStart);
            }

            var result = new AutoCorrectionResult(false);

            foreach (var line in DocumentOperations.GetLines(input.Range))
            {
                if (line.Text.StartsWith("# "))
                {
                    result = InsertHeadline(line, 1);
                }
                else if (line.Text.StartsWith("## "))
                {
                    result = InsertHeadline(line, 2);
                }
                else if (line.Text.StartsWith("### "))
                {
                    result = InsertHeadline(line, 3);
                }
            }

            return result;
        }

        private AutoCorrectionResult InsertHeadline(TextRange line, int level)
        {
            var text = line.Text.Substring(level).Trim();
            line.Text = string.Empty;

            var headline = new Headline(text, line.Start, level);

            if (string.IsNullOrEmpty(text))
            {
                return new AutoCorrectionResult(true, headline.ContentStart);
            }
            else
            {
                return new AutoCorrectionResult(true);
            }
        }

        public AutoCorrectionResult TryUndo(TextPointer start)
        {
            var headline = start.Parent as Headline;
            if (headline != null && string.IsNullOrEmpty(headline.Text))
            {
                headline.SiblingInlines.Remove(headline);
                var body = new Body("#".PadLeft(headline.Level, '#'), start);
                return new AutoCorrectionResult(true, body.ContentEnd);
            }

            return new AutoCorrectionResult(false);
        }
    }
}
