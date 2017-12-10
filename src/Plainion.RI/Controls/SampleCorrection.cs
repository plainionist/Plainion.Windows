using System.Windows.Documents;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.RI.Controls
{
    internal class SampleCorrection : IAutoCorrection
    {
        private const string CopyrightSymbol = "\u00A9";

        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            bool success = false;

            foreach (var wordRange in DocumentOperations.GetWords(input.Range))
            {
                if (wordRange.Text == "(c)")
                {
                    wordRange.Text = CopyrightSymbol;
 
                    success = true;
                }
            }

            return new AutoCorrectionResult(success);
        }

        public AutoCorrectionResult TryUndo(TextPointer pos)
        {
            var word = DocumentOperations.GetWordAt(pos);

            if (word.Text == CopyrightSymbol)
            {
                word.Text = "(c)";
                return new AutoCorrectionResult(true);
            }

            return new AutoCorrectionResult(false);
        }
    }
}