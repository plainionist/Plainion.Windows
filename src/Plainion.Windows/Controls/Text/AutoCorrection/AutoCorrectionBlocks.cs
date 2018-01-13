using System;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class Headline : Run
    {
        private Lazy<int> myLevel;

        public Headline()
            : this(string.Empty)
        {
        }

        public Headline(string text)
            : this(text, null, 2)
        {
        }

        internal Headline(string text, TextPointer insertionPosition, int level)
            : base(text, insertionPosition)
        {
            myLevel = new Lazy<int>(GetLevel);

            FontFamily = TextStyles.Headlines[level - 1].FontFamily;
            FontSize = TextStyles.Headlines[level - 1].FontSize;
            FontWeight = TextStyles.Headlines[level - 1].FontWeight;
            Tag = "Plainion.Text.Headline";
        }

        private int GetLevel()
        {
            if (FontSize == TextStyles.Headlines[0].FontSize)
            {
                return 1;
            }
            else if (FontSize == TextStyles.Headlines[1].FontSize)
            {
                return 2;
            }
            else if (FontSize == TextStyles.Headlines[2].FontSize)
            {
                return 3;
            }
            else
            {
                // default
                return 2;
            }
        }

        public int Level { get { return myLevel.Value; } }
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
