using System.Windows.Documents;
using NUnit.Framework;
using Plainion.Windows.Controls.Text;

namespace Plainion.Windows.Tests.Controls.Text
{
    [RequiresSTA]
    [TestFixture]
    class RichTextEditorSpec
    {
        [Test]
        public void ContainsMultipleFindings_SearchModeAll_AllFindingsHighlighted()
        {
            var editor = new RichTextEditor();
            editor.Document = new FlowDocument(new Paragraph(new Run("f# is concise. F# is functional. F# is great ;-)")));

            var result = editor.Search("f#", SearchMode.All);

            Assert.That(result, Is.True);
        }
    }
}
