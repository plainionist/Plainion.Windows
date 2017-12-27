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
            Corrections.Add(new BulletList());
        }

        public IList<IAutoCorrection> Corrections { get; private set; }

        public AutoCorrectionResult Apply(AutoCorrectionInput input)
        {
            var result = new AutoCorrectionResult(false);

            foreach (var correction in Corrections)
            {
                // could be a paste operation so replacing multiple autocorrections makes sense!
                result = result.Merge(correction.TryApply(input));
            }

            return result;
        }

        public AutoCorrectionResult Undo(TextPointer pos)
        {
            foreach (var correction in Corrections)
            {
                var result = correction.TryUndo(pos);
                if (result.Success)
                {
                    return result;
                }
            }

            return new AutoCorrectionResult(false);
        }
    }
}