using System.Diagnostics;
using System.Windows.Documents;
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
                    wordRange.Text = wordRange.Text.Replace("==>", "-->");

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