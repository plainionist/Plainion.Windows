namespace Plainion.Windows.Specs.Controls.Text

open System.Windows.Documents
open Plainion.Windows.Controls.Text
open Plainion.Windows.Controls.Text.AutoCorrection
open NUnit.Framework
open FsUnit

[<Spec>]
module ``Given a text with potential hyperlinks`` =

    let tryMakeHyperlink (doc:FlowDocument) =
        (new ClickableHyperlink()).TryApply(new TextRange(doc.ContentEnd, doc.ContentEnd))

    let getHyperlinks (doc:FlowDocument) =
        let visitor = new FlowDocumentVisitor(fun e -> e :? Hyperlink)
        visitor.Accept(doc)
        visitor.Results |> Seq.cast<Hyperlink> |> List.ofSeq

    let text (hyperlink:Hyperlink) = 
        let run = hyperlink.Inlines |> Seq.cast<Run> |> Seq.head 
        run.Text

    [<Test>]
    let ``<When> valid URI is entered <Then> URI is replaced with clickable hyperlink``([<Values("http://github.com/", "https://github.com/", "ftp://github.com/")>] url) =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy " + url)))

        document |> tryMakeHyperlink |> should be True

        let links = document |> getHyperlinks
        
        links.Length |> should equal 1

        let hyperlink = links |> Seq.head
        hyperlink |> text |> should equal url
        hyperlink.NavigateUri.ToString() |> should equal url

    [<Test>]
    let ``<When> invalid URI is entered <Then> no hyperlink is inserted`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy text")));

        document |> tryMakeHyperlink |> should be False

        document |> getHyperlinks |> should be Empty

    [<Test>]
    let ``<When> URI without protocol but with "www" is entered <Then> clickable hyperlink is created`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy www.host.org")));

        document |> tryMakeHyperlink |> should be True

        let links = document |> getHyperlinks
        
        links.Length |> should equal 1

        let hyperlink = links |> Seq.head
        hyperlink |> text |> should equal "www.host.org"
        hyperlink.NavigateUri.ToString() |> should equal "http://www.host.org/"

    [<Test>]
    let ``<When> removal of hyperlink triggered <Then> clickable hyperlink is removed`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy http://github.org/")));

        document |> tryMakeHyperlink |> should be True
        (new ClickableHyperlink()).TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) |> should be True

        document |> getHyperlinks |> should be Empty
