using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkdownHeadline : IAutoCorrection
    {
        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            // add a new body Run if user hit enter after existing headline
            if(input.Trigger == AutoCorrectionTrigger.Return && input.Range.End.Parent is Headline)
            {
                var body = new Body(string.Empty, input.Range.End);
                return new AutoCorrectionResult(true, body.ContentStart);
            }

            var result = new AutoCorrectionResult(false);

            foreach (var line in DocumentOperations.GetLines(input.Range))
            {
                if (line.Text.StartsWith("# "))
                {
                    result = InsertHeadline(line, 0);
                }
                else if (line.Text.StartsWith("## "))
                {
                    result = InsertHeadline(line, 1);
                }
                else if (line.Text.StartsWith("### "))
                {
                    result = InsertHeadline(line, 2);
                }
            }

            return result;
        }

        private AutoCorrectionResult InsertHeadline(TextRange line, int level)
        {
            var text = line.Text.Substring(level + 1).Trim();
            line.Text = string.Empty;

            var headline = new Headline(text, line.Start, TextStyles.Headlines[level]);

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
            return new AutoCorrectionResult(false);
        }
    }
}
