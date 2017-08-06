namespace Plainion.Windows.Specs.Controls.Text

open System
open System.Windows.Documents
open NUnit.Framework
open Plainion.Windows.Controls.Text
open System.Threading

[<Apartment(ApartmentState.STA)>]
[<TestFixture>]
module RichTextEditorSpecs =

    let equalsI (lhs:string) rhs = lhs.Equals(rhs, StringComparison.OrdinalIgnoreCase)

    let searchResults doc = 
        let visitor = new FlowDocumentVisitor(fun e -> e.GetType() = typeof<Run>)
        visitor.Accept(doc)

        visitor.Results
        |> Seq.cast<Run>
        |> Seq.filter(fun e -> e.Background = RichTextEditor.SearchHighlightBrush)
        |> List.ofSeq

    let count n (results:Run list) =
        Assert.That(results.Length, Is.EqualTo(n))

    let at offset (results:Run list) =
        let head = results |> Seq.head

        let container = head.Parent :?> Block
        Assert.That(container.ContentStart.GetOffsetToPosition(head.ContentStart), Is.EqualTo(offset))

    let shouldMatch searchText f (results:Run list) =
        Assert.That(results |> Seq.forall(fun x -> equalsI x.Text searchText), Is.True)
        results |> f

    [<Test>]
    let ``When searching for all occurences Then all words should be highlighted``() =
        let editor = new RichTextEditor()
        editor.Document <- new FlowDocument(new Paragraph(new Run("f# is concise. F# is functional. F# is great ;-)")))

        let result = editor.Search("f#", SearchMode.All)
        Assert.That(result, Is.True)

        editor.Document |> searchResults |> shouldMatch "f#" (count 3)

    [<Test>]
    let ``When searching forward and backward Then correct offsets should be found``() =
        let editor = new RichTextEditor()
        editor.Document <- new FlowDocument(new Paragraph(new Run("f# is concise." + Environment.NewLine + "F# is functional. F# is great ;-)")))

        Assert.That(editor.Search("f#", SearchMode.Initial), Is.True)

        editor.Document |> searchResults |> shouldMatch "f#" (at 1)
        
        Assert.That(editor.Search("f#", SearchMode.Next), Is.True)

        editor.Document |> searchResults |> shouldMatch "f#" (at 19)

        Assert.That(editor.Search("f#", SearchMode.Next), Is.True)

        editor.Document |> searchResults |> shouldMatch "f#" (at 37)

        Assert.That(editor.Search("f#", SearchMode.Previous), Is.True)

        editor.Document |> searchResults |> shouldMatch "f#" (at 19)

        Assert.That(editor.Search("f#", SearchMode.Previous), Is.True)

        editor.Document |> searchResults |> shouldMatch "f#" (at 1)
