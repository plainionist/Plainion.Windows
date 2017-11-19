using System.Windows.Documents;
using System.Windows.Media;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.RI.Controls
{
    internal class ArrowCorrection : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            bool ret = false;

            foreach (var wordRange in DocumentOperations.GetWords(range))
            {
                if (wordRange.Text == "==>")
                {
                    wordRange.Text = wordRange.Text.Replace("==>", "");

                    var run = new Run("\u21e8", wordRange.End);
                    run.FontFamily = new FontFamily("Segoe UI Symbol");
                    run.Foreground = Brushes.Red;
                    run.Tag = "==>";
 
                    ret = true;
                }
            }

            return ret;
        }

        public TextPointer TryUndo(TextPointer start)
        {
            return null;
        }
    }
}