using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class BulletList : IAutoCorrection
    {
        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            var result = new AutoCorrectionResult(false);

            foreach (var line in input.Context.GetLines())
            {
                if (line.Text.StartsWith("- "))
                {
                    result = InsertBullet(input, line, 0);
                }
            }

            return result;
        }

        private AutoCorrectionResult InsertBullet(AutoCorrectionInput input, TextRange line, int level)
        {
            line.Text = line.Text.Trim().Substring(1).Trim();

            input.Editor.ToggleBulletsCommand.Execute(null);

            return new AutoCorrectionResult(true);
        }

        public AutoCorrectionResult TryUndo(TextPointer start)
        {
            // handled with RichTextBox private implementation
            return new AutoCorrectionResult(false);
        }
    }
}
