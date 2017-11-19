using System.Collections.Generic;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class AutoCorrectionTable
    {
        public AutoCorrectionTable()
        {
            Corrections = new List<IAutoCorrection>();
            Corrections.Add(new ClickableHyperlink());
            Corrections.Add(new MarkupHeadline());
        }

        public IList<IAutoCorrection> Corrections { get; private set; }

        public void Apply(TextRange range)
        {
            foreach (var correction in Corrections)
            {
                if (correction.TryApply(range))
                {
                    break;
                }
            }
        }

        public TextPointer Undo(TextPointer start)
        {
            foreach (var correction in Corrections)
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