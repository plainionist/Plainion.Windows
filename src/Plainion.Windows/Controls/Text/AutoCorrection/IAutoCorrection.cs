using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public interface IAutoCorrection
    {
        bool TryApply(TextRange range);
        bool TryUndo(TextPointer pos);
    }
}
