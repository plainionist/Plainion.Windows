using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class MarkdownHeadline : IAutoCorrection
    {
        public bool TryApply(TextRange range)
        {
            bool ret = false;

            //var lineStart = DocumentOperations.GetLineAt(range.Start).Start;

            //foreach (var wordRange in DocumentOperations.GetWords(range))
            //{
            //    if (wordRange.Start.CompareTo(lineStart) == 0  && wordRange.Text == "##") 
            //    {
            //        wordRange.Text = string.Empty;
            //        wordRange.ApplyPropertyValue(Inline.FontSizeProperty, TextStyles.Headline.FontSize);
            //        wordRange.ApplyPropertyValue(Inline.FontWeightProperty, TextStyles.Headline.FontWeight);

            //        ret = true;
            //    }
            //}

            return ret;
        }

        public bool TryUndo(TextPointer start)
        {
            return false;
        }
    }
}
