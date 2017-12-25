using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class Headline : Run
    {
        public Headline()
            : this(string.Empty, null, TextStyles.Headlines[1])
        {
        }

        internal Headline(string text, TextPointer insertionPosition, TextStyles.Headline headline)
            : base(text, insertionPosition)
        {
            FontFamily = headline.FontFamily;
            FontSize = headline.FontSize;
            FontWeight = headline.FontWeight;
            Tag = GetType().FullName;
        }
    }

    public class Body : Run
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
