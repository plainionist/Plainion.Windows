using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public class UnicodeSymbolCorrection : IAutoCorrection
    {
        public class Symbol
        {
            public Symbol(string ascii, string uniCode)
            {
                Ascii = ascii;
                UniCode = uniCode;
            }

            public string Ascii { get; private set; }
            public string UniCode { get; private set; }
        }

        public UnicodeSymbolCorrection()
        {
            Symbols = new List<Symbol>();
            Symbols.Add(new Symbol("-->", "\u2192"));
            Symbols.Add(new Symbol("==>", "\u2794"));
        }

        public IList<Symbol> Symbols { get; private set; }

        public AutoCorrectionResult TryApply(AutoCorrectionInput input)
        {
            bool success = false;

            foreach (var wordRange in input.Context.GetWords())
            {
                var symbol = Symbols.FirstOrDefault(x => x.Ascii == wordRange.TextOnly());
                if (symbol != null)
                {
                    wordRange.Text = symbol.UniCode;

                    success = true;
                }
            }

            return new AutoCorrectionResult(success);
        }

        public AutoCorrectionResult TryUndo(TextPointer pos)
        {
            var wordRange = DocumentOperations.GetWordAt(pos);
            var symbol = Symbols.FirstOrDefault(x => x.UniCode == wordRange.TextOnly());
            if (symbol != null)
            {
                wordRange.Text = symbol.Ascii;
                return new AutoCorrectionResult(true);
            }

            return new AutoCorrectionResult(false);
        }
    }
}
