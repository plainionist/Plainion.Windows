using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text.AutoCorrection
{
    public interface IAutoCorrection
    {
        AutoCorrectionResult TryApply(AutoCorrectionInput input);
        AutoCorrectionResult TryUndo(TextPointer pos);
    }

    public enum AutoCorrectionTrigger
    {
        Return,
        Space,
        CopyAndPaste
    }

    public class AutoCorrectionInput
    {
        public AutoCorrectionInput(TextRange range, AutoCorrectionTrigger trigger)
        {
            Contract.RequiresNotNull(range, "range");

            Range = range;
            Trigger = trigger;
            Context = new AutoCorrectionContext(range);
        }

        public TextRange Range { get; private set; }

        public AutoCorrectionTrigger Trigger { get; private set; }

        public IAutoCorrectionContext Context { get; private set; }

        internal RichTextEditor Editor { get; set; }
    }

    public interface IAutoCorrectionContext
    {
        IReadOnlyCollection<TextRange> GetLines();
        IReadOnlyCollection<TextRange> GetWords();
    }

    class AutoCorrectionContext : IAutoCorrectionContext
    {
        private TextRange myRange;
        private Lazy<IReadOnlyCollection<TextRange>> myLines;
        private Lazy<IReadOnlyCollection<TextRange>> myWords;

        public AutoCorrectionContext(TextRange range)
        {
            myRange = range;

            myLines = new Lazy<IReadOnlyCollection<TextRange>>(() => DocumentOperations.GetLines(range));
            myWords = new Lazy<IReadOnlyCollection<TextRange>>(() => DocumentOperations.GetWords(range));
        }

        public IReadOnlyCollection<TextRange> GetLines()
        {
            return myLines.Value;
        }

        public IReadOnlyCollection<TextRange> GetWords()
        {
            return myWords.Value;
        }
    }

    public class AutoCorrectionResult
    {
        public AutoCorrectionResult(bool success)
        : this(success, null)
        {
        }

        public AutoCorrectionResult(bool success, TextPointer caretPosition)
        {
            Success = success;
            CaretPosition = caretPosition;
        }

        public bool Success { get; private set; }

        public TextPointer CaretPosition { get; private set; }

        internal AutoCorrectionResult Merge(AutoCorrectionResult other)
        {
            Contract.RequiresNotNull(other, "other");

            return new AutoCorrectionResult(
                Success || other.Success,
                GetCaretPosition(CaretPosition, other.CaretPosition)
                );
        }

        private static TextPointer GetCaretPosition(TextPointer lhs, TextPointer rhs)
        {
            if (lhs == null)
            {
                return rhs;
            }
            if (rhs == null)
            {
                return lhs;
            }

            return lhs.CompareTo(rhs) > 0 ? lhs : rhs;
        }
    }
}
