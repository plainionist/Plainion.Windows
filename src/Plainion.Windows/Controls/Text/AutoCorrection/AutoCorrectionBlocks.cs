using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    internal class Headline : Run
    {
        public Headline()
            : this(string.Empty, null, TextStyles.Headlines[1])
        {
        }

        public Headline(string text, TextPointer insertionPosition, TextStyles.Headline headline)
            : base(text, insertionPosition)
        {
            FontFamily = headline.FontFamily;
            FontSize = headline.FontSize;
            FontWeight = headline.FontWeight;
            Tag = GetType().FullName;
        }

        public static bool IsHeadline(TextPointer pos)
        {
            var parent = pos.Parent as Run;
            return parent != null && parent.Tag != null && parent.Tag.Equals(typeof(Headline).FullName);
        }
    }

    internal class Body : Run
    {
        public Body()
            : this(string.Empty, null)
        {
        }

        public Body(string text, TextPointer insertionPosition)
            : base(text, insertionPosition)
        {
            FontFamily = TextStyles.Body.FontFamily;
            FontSize = TextStyles.Body.FontSize;
            FontWeight = TextStyles.Body.FontWeight;
        }
    }

}
