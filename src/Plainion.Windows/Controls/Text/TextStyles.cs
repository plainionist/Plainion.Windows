using System.Windows;
using System.Windows.Media;

namespace Plainion.Windows.Controls.Text
{
    class TextStyles
    {
        // https://blogs.msdn.microsoft.com/text/2009/12/11/wpf-text-measurement-units/
        private static double pt = 96.0 / 72.0;

        public class Body
        {
            public static FontFamily FontFamily = new FontFamily("Calibri");
            public static double FontSize = 11.0 * pt;
            public static FontWeight FontWeight = FontWeights.Normal;
        }

        public class Headline
        {
            public static FontFamily FontFamily = new FontFamily("Calibri");
            public static double FontSize = 14.0 * pt;
            public static FontWeight FontWeight = FontWeights.Bold;
        }
    }
}
