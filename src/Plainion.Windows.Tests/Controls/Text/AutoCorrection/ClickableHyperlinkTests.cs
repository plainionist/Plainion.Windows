using System.Linq;
using System.Windows.Documents;
using NUnit.Framework;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;

namespace Plainion.Windows.Tests.Controls.Text.AutoCorrection
{
    [TestFixture]
    class ClickableHyperlinkTests
    {
        [Test]
        public void TryApply_AfterLink_HyperlinkInserted([Values("http://github.com/", "https://github.com/", "ftp://github.com/")]string url)
        {
            var document = new FlowDocument(new Paragraph(new Run("Some dummy " + url)));

            new ClickableHyperlink().TryApply(new TextRange(document.ContentEnd, document.ContentEnd));

            var visitor = new FlowDocumentVisitor(e => e is Hyperlink);
            visitor.Accept(document);
            Assert.That(visitor.Results.Count, Is.EqualTo(1));

            var hyperlink = visitor.Results.OfType<Hyperlink>().Single();
            Assert.That(hyperlink.Inlines.OfType<Run>().Single().Text, Is.EqualTo(url));
            Assert.That(hyperlink.NavigateUri.ToString(), Is.EqualTo(url));
        }

        [Test]
        public void TryApply_AfterNonLink_NoHyperlinkInserted()
        {
            var document = new FlowDocument(new Paragraph(new Run("Some dummy text")));

            new ClickableHyperlink().TryApply(new TextRange(document.ContentEnd, document.ContentEnd));

            var visitor = new FlowDocumentVisitor(e => e is Hyperlink);
            visitor.Accept(document);
            Assert.That(visitor.Results, Is.Empty);
        }

        [Test]
        public void TryApply_AfterIncompleteWwwLink_HyperlinkWithHttpPrefixInserted()
        {
            var document = new FlowDocument(new Paragraph(new Run("Some dummy www.host.org")));

            new ClickableHyperlink().TryApply(new TextRange(document.ContentEnd, document.ContentEnd));

            var visitor = new FlowDocumentVisitor(e => e is Hyperlink);
            visitor.Accept(document);
            Assert.That(visitor.Results.Count, Is.EqualTo(1));

            var hyperlink = visitor.Results.OfType<Hyperlink>().Single();
            Assert.That(hyperlink.Inlines.OfType<Run>().Single().Text, Is.EqualTo("www.host.org"));
            Assert.That(hyperlink.NavigateUri.ToString(), Is.EqualTo("http://www.host.org/"));
        }


        [Test]
        public void TryUndo_AfterLink_HyperlinkRemoved()
        {
            var document = new FlowDocument(new Paragraph(new Run("Some dummy http://github.org/")));

            new ClickableHyperlink().TryApply(new TextRange(document.ContentEnd, document.ContentEnd));
            new ClickableHyperlink().TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward));

            var visitor = new FlowDocumentVisitor(e => e is Hyperlink);
            visitor.Accept(document);
            Assert.That(visitor.Results, Is.Empty);
        }
    }
}
