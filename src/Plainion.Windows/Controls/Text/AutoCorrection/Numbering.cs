using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class Numbering : IAutoCorrection
    {
        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            var result = new AutoCorrectionResult(false);

            foreach (var line in input.Context.GetLines())
            {
                if (line.Text.StartsWith("1. "))
                {
                    result = InsertNumbering(input, line, 0);
                }
            }

            return result;
        }

        private AutoCorrectionResult InsertNumbering(AutoCorrectionInput input, TextRange line, int level)
        {
            // remove "1."
            line.Text = line.Text.Trim().Substring(2).Trim();

            input.Editor.ToggleNumberingommand.Execute(null);

            return new AutoCorrectionResult(true);
        }

        public AutoCorrectionResult TryUndo(TextPointer start)
        {
            // handled with RichTextBox private implementation
            return new AutoCorrectionResult(false);
        }
    }
}
