using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using NUnit.Framework;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.Windows.Tests.Controls.Text
{
    [Apartment(ApartmentState.STA)]
    [TestFixture]
    class RichTextEditorTests
    {
        private class AutoCorrectionObserver : IAutoCorrection
        {
            public bool ApplyTriggered { get; private set; }
            public bool UndoTriggered { get; private set; }

            public AutoCorrectionResult TryApply(AutoCorrectionInput input)
            {
                ApplyTriggered = true;
                return new AutoCorrectionResult(false);
            }

            public AutoCorrectionResult TryUndo(TextPointer pos)
            {
                UndoTriggered = true;
                return new AutoCorrectionResult(false);
            }
        }

        private RichTextEditor myEditor;
        private AutoCorrectionObserver myAutoCorrections;

        [SetUp]
        public void SetUp()
        {
            myEditor = new RichTextEditor();
            myEditor.AutoCorrection.Corrections.Clear();

            myAutoCorrections = new AutoCorrectionObserver();
            myEditor.AutoCorrection.Corrections.Add(myAutoCorrections);
        }

        [Test]
        public void OnKeyDown_WordCompletionCharacter_AutoCorrectionTriggered([Values(Key.Space, Key.Return)]Key key)
        {
            AddText(new Paragraph(new Run("Some dummy text")));

            myEditor.TriggerInput(key);

            Assert.That(myAutoCorrections.ApplyTriggered, Is.True);
        }

        [Test]
        public void OnKeyDown_WordContinued_NoAutoCorrectionTriggered()
        {
            AddText(new Paragraph(new Run("Some dummy text")));

            myEditor.TriggerInput(Key.A);

            Assert.That(myAutoCorrections.ApplyTriggered, Is.False);
        }

        [Test]
        public void OnKeyDown_Backspace_UndoOfAutoCorrectionTriggered()
        {
            AddText(new Paragraph(new Run("Some dummy http://github.org/")));

            myEditor.TriggerInput(Key.Back);

            Assert.That(myAutoCorrections.UndoTriggered, Is.True);
        }

        [Test]
        public void OnPaste_WhenCalled_AutoCorrectionTriggered()
        {
            var url = "http://github.com/";

            AddText(new Paragraph(new Run("Some dummy ")));

            Clipboard.SetData(DataFormats.Text, url);
            myEditor.Paste();

            Assert.That(myAutoCorrections.UndoTriggered, Is.False);
        }

        private void AddText(Paragraph text)
        {
            myEditor.Document.Blocks.Add(text);
            myEditor.CaretPosition = myEditor.Document.ContentEnd;
        }
    }
}
