namespace Plainion.Windows.Specs.Controls.Text

open System.Windows.Documents
open Plainion.Windows.Controls.Text
open Plainion.Windows.Controls.Text.AutoCorrection
open NUnit.Framework
open FsUnit

[<Spec>]
module ``Given a text to be auto-corrected (hyperlinks)`` =

    let tryAutoCorrect (doc:FlowDocument) =
        (new ClickableHyperlink()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return))

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

        let result = document |> tryAutoCorrect 
        result.Success |> should be True

        let links = document |> getHyperlinks
        
        links.Length |> should equal 1

        let hyperlink = links |> Seq.head
        hyperlink |> text |> should equal url
        hyperlink.NavigateUri.ToString() |> should equal url

    [<Test>]
    let ``<When> invalid URI is entered <Then> no hyperlink is inserted`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy text")));

        let result = document |> tryAutoCorrect 
        
        result.Success |> should be False

        document |> getHyperlinks |> should be Empty

    [<Test>]
    let ``<When> URI without protocol but with "www" is entered <Then> clickable hyperlink is created`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy www.host.org")));

        let result = document |> tryAutoCorrect 
        
        result.Success |> should be True

        let links = document |> getHyperlinks
        
        links.Length |> should equal 1

        let hyperlink = links |> Seq.head
        hyperlink |> text |> should equal "www.host.org"
        hyperlink.NavigateUri.ToString() |> should equal "http://www.host.org/"

    [<Test>]
    let ``<When> removal of hyperlink is triggered <Then> clickable hyperlink is removed`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy http://github.org/")));

        document |> tryAutoCorrect |> ignore
        let result = (new ClickableHyperlink()).TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) 
        result.Success |> should be True

        document |> getHyperlinks |> should be Empty

[<Spec>]
module ``Given a text to be auto-corrected (unicode symbol)`` =

    let tryAutoCorrect (doc:FlowDocument) =
        (new UnicodeSymbolCorrection()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return))

    let text (doc:FlowDocument) = 
        let range = new TextRange(doc.ContentStart, doc.ContentEnd)
        range.Text

    [<Test>]
    let ``<When> "-->" is enteried <Then> it will be replaced by arrow symbol`` () =
        let document = new FlowDocument(new Paragraph(new Run("so -->")))

        let result = document |> tryAutoCorrect 
        
        result.Success |> should be True

        document |> text |> should haveSubstring "\u2192"

    [<Test>]
    let ``<When> "==>" is enteried <Then> it will be replaced by arrow symbol`` () =
        let document = new FlowDocument(new Paragraph(new Run("so ==>")))

        let result = document |> tryAutoCorrect 
        
        result.Success |> should be True

        document |> text |> should haveSubstring "\u21e8"

    [<Test>]
    let ``<When> unrecognized text is entered <Then> no symbol is inserted`` () =
        let document = new FlowDocument(new Paragraph(new Run("Some dummy text")));

        let result = document |> tryAutoCorrect 
        
        result.Success |> should be False

        document |> text |> should equal "Some dummy text\r\n"

    [<Test>]
    let ``<When> removal of symbol is triggered <Then> symbol is removed`` () =
        let document = new FlowDocument(new Paragraph(new Run("we conclude ==>")))

        document |> tryAutoCorrect |> ignore
        let result = (new UnicodeSymbolCorrection()).TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward))
        result.Success |> should be True

        document |> text |> should equal "we conclude ==>\r\n"

//[<Spec>]
//module ``Given a text to be auto-corrected (markdown headline)`` =

//    let tryAutoCorrect (doc:FlowDocument) =
//        (new MarkdownHeadline()).TryApply(new TextRange(doc.ContentEnd, doc.ContentEnd))

//    let text (doc:FlowDocument) = 
//        let range = new TextRange(doc.ContentStart, doc.ContentEnd)
//        range.Text

//    [<Test>]
//    let ``<When> "-->" is enteried <Then> it will be replaced by arrow symbol`` () =
//        let document = new FlowDocument(new Paragraph(new Run("so -->")))

//        document |> tryAutoCorrect |> should be True

//        document |> text |> should haveSubstring "\u2192"

//    [<Test>]
//    let ``<When> "==>" is enteried <Then> it will be replaced by arrow symbol`` () =
//        let document = new FlowDocument(new Paragraph(new Run("so ==>")))

//        document |> tryAutoCorrect |> should be True

//        document |> text |> should haveSubstring "\u21e8"

//    [<Test>]
//    let ``<When> unrecognized text is entered <Then> no symbol is inserted`` () =
//        let document = new FlowDocument(new Paragraph(new Run("Some dummy text")));

//        document |> tryAutoCorrect |> should be False

//        document |> text |> should equal "Some dummy text\r\n"

//    [<Test>]
//    let ``<When> removal of symbol is triggered <Then> symbol is removed`` () =
//        let document = new FlowDocument(new Paragraph(new Run("we conclude ==>")))

//        document |> tryAutoCorrect |> should be True
//        (new MarkdownHeadline()).TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) |> should be True

//        document |> text |> should equal "we conclude ==>\r\n"
