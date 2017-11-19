using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkupHeadline : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            bool ret = false;

            foreach (var wordRange in DocumentOperations.GetWords(range))
            {
                if (wordRange.Text == "##") // TODO: at line start
                {
                    wordRange.Text = wordRange.Text.Replace("##", "");
                    // TODO: change font

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
