using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class Headline : Run
    {
        public Headline()
            : this(string.Empty)
        {
        }

        public Headline(string text)
            : this(text, null, TextStyles.Headlines[1])
        {
        }

        internal Headline(string text, TextPointer insertionPosition, TextStyles.Headline headline)
            : base(text, insertionPosition)
        {
            FontFamily = headline.FontFamily;
            FontSize = headline.FontSize;
            FontWeight = headline.FontWeight;
            Tag = "Plainion.Text.Headline";
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
            Tag = "Plainion.Text.Body";
        }
    }

}
