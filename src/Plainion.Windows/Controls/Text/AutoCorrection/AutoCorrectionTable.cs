using System.Collections.Generic;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class AutoCorrectionTable
    {
        private List<IAutoCorrection> myCorrections;

        public AutoCorrectionTable()
        {
            myCorrections = new List<IAutoCorrection>();
            myCorrections.Add(new ClickableHyperlink());
        }

        public void Apply(TextRange range)
        {
            foreach (var correction in myCorrections)
            {
                if (correction.TryApply(range))
                {
                    break;
                }
            }
        }

        public TextPointer Undo(TextPointer start)
        {
            foreach (var correction in myCorrections)
            {
                var newCaretPosition = correction.TryUndo(start);
                if (newCaretPosition != null)
                {
                    return newCaretPosition;
                }
            }

            return null;
        }
    }
}