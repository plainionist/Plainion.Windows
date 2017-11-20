using System.Windows.Documents;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.RI.Controls
{
    internal class SampleCorrection : IAutoCorrection
    {
        private const string CopyrightSymbol = "\u00A9";

        public bool TryApply(TextRange range)
        {
            bool ret = false;

            foreach (var wordRange in DocumentOperations.GetWords(range))
            {
                if (wordRange.Text == "(c)")
                {
                    wordRange.Text = CopyrightSymbol;
 
                    ret = true;
                }
            }

            return ret;
        }

        public bool TryUndo(TextPointer pos)
        {
            var word = DocumentOperations.GetWordAt(pos);

            if (word.Text == CopyrightSymbol)
            {
                word.Text = "(c)";
                return true;
            }

            return false;
        }
    }
}