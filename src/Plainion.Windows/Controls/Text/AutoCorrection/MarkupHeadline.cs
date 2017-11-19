using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkupHeadline : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            return false;
        }

        public TextPointer TryUndo(TextPointer start)
        {
            return null;
        }
    }
}
