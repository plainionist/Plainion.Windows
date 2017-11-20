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
            Corrections.Add(new UnicodeSymbolCorrection());
            Corrections.Add(new MarkdownHeadline());
        }

        public IList<IAutoCorrection> Corrections { get; private set; }

        public bool Apply(TextRange range)
        {
            foreach (var correction in Corrections)
            {
                if (correction.TryApply(range))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Undo(TextPointer pos)
        {
            foreach (var correction in Corrections)
            {
                if (correction.TryUndo(pos))
                {
                    return true;
                }
            }

            return false;
        }
    }
}