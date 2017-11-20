using System.Linq;
using System.Windows.Documents;
using NUnit.Framework;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.Windows.Tests.Controls.Text.AutoCorrection
{
    [TestFixture]
    class DocumentOperationsTests
    {
        private FlowDocument myDocument;

        [SetUp]
        public void SetUp()
        {
            var body = new Paragraph();
            body.Inlines.Add(new Run("Here is some text to test parsing"));
            body.Inlines.Add(new LineBreak());
            body.Inlines.Add(new Run("a second line would be helpful."));
            body.Inlines.Add(new LineBreak());
            body.Inlines.Add(new Run("A third is even better."));

            myDocument = new FlowDocument(body);
        }

        [Test]
        public void GetPointerFromCharOffset_BeginningOfRange_ReturnsRangeStart()
        {
            var pos = DocumentOperations.GetPointerFromCharOffset(new TextRange(myDocument.ContentStart, myDocument.ContentEnd), 0);
            Assert.That(new TextRange(pos, pos.GetPositionAtOffset(5)).Text, Is.EqualTo("Here "));
        }

        [Test]
        public void GetPointerFromCharOffset_AtBeginningOfWord_ReturnsBeginningOfWord()
        {
            var pos = DocumentOperations.GetPointerFromCharOffset(new TextRange(myDocument.ContentStart, myDocument.ContentEnd), 8);
            Assert.That(new TextRange(pos, pos.GetPositionAtOffset(5)).Text, Is.EqualTo("some "));
        }

        [Test]
        public void GetPointerFromCharOffset_AtEndOfLine_ReturnsEndOfLine()
        {
            var pos = DocumentOperations.GetPointerFromCharOffset(new TextRange(myDocument.ContentStart, myDocument.ContentEnd), 33);
            Assert.That(new TextRange(pos, pos.GetPositionAtOffset(9)).Text, Is.EqualTo("\r\na sec"));
        }

        [Test]
        public void GetWord_AtBeginningOfWord_ReturnsThatWord()
        {
            var pos = DocumentOperations.GetPointerFromCharOffset(new TextRange(myDocument.ContentStart, myDocument.ContentEnd), 8);
            Assert.That(DocumentOperations.GetWord(pos).Text, Is.EqualTo("some"));
        }

        [Test]
        public void GetWord_InsideWord_ReturnsThatWord()
        {
            var pos = DocumentOperations.GetPointerFromCharOffset(new TextRange(myDocument.ContentStart, myDocument.ContentEnd), 10);
            Assert.That(DocumentOperations.GetWord(pos).Text, Is.EqualTo("some"));
        }

        [Test]
        public void GetWord_EndOfWord_ReturnsThatWord()
        {
            var pos = DocumentOperations.GetPointerFromCharOffset(new TextRange(myDocument.ContentStart, myDocument.ContentEnd), 12);
            Assert.That(DocumentOperations.GetWord(pos).Text, Is.EqualTo("some"));
        }
    }
}
