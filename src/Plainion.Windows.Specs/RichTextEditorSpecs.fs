namespace Plainion.Windows.Specs.Controls.Text

open System.Windows.Documents
open NUnit.Framework
open Plainion.Windows.Controls.Text
open System.Threading

[<Apartment(ApartmentState.STA)>]
[<TestFixture>]
module RichTextEditorSpecs =

    [<Test>]
    let ``Given same word multiple times - When SearchMode.All - Then all workds should be highlighted``() =
        let editor = new RichTextEditor();
        editor.Document <- new FlowDocument(new Paragraph(new Run("f# is concise. F# is functional. F# is great ;-)")));

        let result = editor.Search("f#", SearchMode.All);

        Assert.That(result, Is.True);
