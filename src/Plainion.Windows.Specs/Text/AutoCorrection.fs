[<Feature(InOrderTo = "create nicely formated content driven documents",
          AsA="author", 
          IWantTo = "format and structure text using automatic corrections")>]
module Plainion.Windows.Text.AutoCorrection

open System.Windows.Documents
open NUnit.Framework
open FsUnit
open Plainion.Windows.Controls.Text
open Plainion.Windows.Controls.Text.AutoCorrection

let simulateReturn (doc:FlowDocument) =
    new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return)

let success (result:AutoCorrectionResult) = result.Success

let getHyperlinks (doc:FlowDocument) =
    let visitor = new FlowDocumentVisitor(fun e -> e :? Hyperlink)
    visitor.Accept(doc)
    visitor.Results |> Seq.cast<Hyperlink> |> List.ofSeq

let text (doc:FlowDocument) = 
    let range = new TextRange(doc.ContentStart, doc.ContentEnd)
    range.Text

let headline (doc:FlowDocument) = 
    let visitor = new FlowDocumentVisitor(fun x -> x :? Headline)
    visitor.Accept(doc)
    visitor.Results |> Seq.cast<Headline> |> Seq.tryHead

[<Scenario(Caption="Auto-correct URLs")>]
module ``Given a text in URI format`` =
    let text (hyperlink:Hyperlink) = 
        let run = hyperlink.Inlines |> Seq.cast<Run> |> Seq.head 
        run.Text

    [<When>]
    let ``<When> it forms a valid URI <Then> URI is replaced with clickable hyperlink``([<Values("http://github.com/", "https://github.com/", "ftp://github.com/")>] url) =
        let doc = new FlowDocument(new Paragraph(new Run("Some dummy " + url)))

        doc 
        |> simulateReturn 
        |> (new ClickableHyperlink()).TryApply 
        |> success
        |> should be True

        let links = doc |> getHyperlinks

        links.Length |> should equal 1

        let hyperlink = links |> Seq.head
        hyperlink |> text |> should equal url
        hyperlink.NavigateUri.ToString() |> should equal url

    [<When>]
    let ``<When> it forms an invalid URI <Then> no hyperlink is inserted`` () =
        let doc = new FlowDocument(new Paragraph(new Run("Some dummy text")));

        doc 
        |> simulateReturn 
        |> (new ClickableHyperlink()).TryApply 
        |> success
        |> should be False

        doc |> getHyperlinks |> should be Empty

    [<When>]
    let ``<When> it is a URI without protocol but with "www" <Then> clickable hyperlink is created`` () =
        let doc = new FlowDocument(new Paragraph(new Run("Some dummy www.host.org")));

        doc 
        |> simulateReturn 
        |> (new ClickableHyperlink()).TryApply 
        |> success
        |> should be True

        let links = doc |> getHyperlinks
        
        links.Length |> should equal 1

        let hyperlink = links |> Seq.head
        hyperlink |> text |> should equal "www.host.org"
        hyperlink.NavigateUri.ToString() |> should equal "http://www.host.org/"

[<Scenario(Caption="Undo auto-corrected URLs")>]
module ``Given a clickable hyperlink`` =
    [<When>]
    let ``<When> BACKSPACE is entered <Then> clickable hyperlink is removed`` () =
        let doc = new FlowDocument(new Paragraph(new Run("Some dummy http://github.org/")));

        doc 
        |> simulateReturn 
        |> (new ClickableHyperlink()).TryApply 
        |> ignore

        let result = (new ClickableHyperlink()).TryUndo(doc.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) 
        result.Success |> should be True

        doc |> getHyperlinks |> should be Empty

[<Scenario(Caption="Auto-correct Ascii symbols")>]
module ``Given a character sequence`` =
    [<When>]
    let ``<When> "-->" is enteried <Then> it will be replaced by arrow symbol`` () =
        let doc = new FlowDocument(new Paragraph(new Run("so -->")))

        doc 
        |> simulateReturn
        |> (new UnicodeSymbolCorrection()).TryApply
        |> success
        |> should be True

        doc |> text |> should haveSubstring "\u2192"

    [<When>]
    let ``<When> "==>" is enteried <Then> it will be replaced by arrow symbol`` () =
        let doc = new FlowDocument(new Paragraph(new Run("so ==>")))

        doc 
        |> simulateReturn
        |> (new UnicodeSymbolCorrection()).TryApply
        |> success
        |> should be True

        doc |> text |> should haveSubstring "\u2794"

    [<When>]
    let ``<When> unknown sequence is entered <Then> no symbol is inserted`` () =
        let doc = new FlowDocument(new Paragraph(new Run("Some dummy text")));

        doc 
        |> simulateReturn
        |> (new UnicodeSymbolCorrection()).TryApply
        |> success
        |> should be False
        
        doc |> text |> should equal "Some dummy text\r\n"

[<Scenario(Caption="Undo auto-correct Ascii symbols")>]
module ``Given a Unicode symbol`` =
    [<When>]
    let ``<When> BACKSPACE is entered <Then> symbol is removed`` () =
        let doc = new FlowDocument(new Paragraph(new Run("we conclude ==>")))

        doc 
        |> simulateReturn
        |> (new UnicodeSymbolCorrection()).TryApply
        |> ignore

        let result = (new UnicodeSymbolCorrection()).TryUndo(doc.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward))
        result.Success |> should be True

        doc |> text |> should equal "we conclude ==>\r\n"

[<Scenario(Caption="Auto-correct MarkDown headlines")>]
module ``Given a text line with leading # characters`` =
    [<When>]
    let ``<When> SPACE is pressed after '#' character <Then> the line will be converted into a headline`` () =
        let doc = new FlowDocument(new Paragraph(new Run("# ")))

        doc 
        |> simulateReturn
        |> (new MarkdownHeadline()).TryApply
        |> success
        |> should be True

        let headline = doc |> headline 
        headline |> should not' (equal None)
        headline |> Option.map(fun x -> x.Text) |> should equal (Some(""))

    [<When>]
    let ``<When> RETURN is pressed after a headline <Then> a new body text block is started`` () =
        let doc = new FlowDocument(new Paragraph(new Headline("headline")))

        doc 
        |> simulateReturn
        |> (new MarkdownHeadline()).TryApply
        |> success
        |> should be True

        doc.Blocks
        |> Seq.cast<Paragraph>
        |> Seq.last
        |> fun b -> b.Inlines
        |> Seq.last
        |> should be instanceOfType<Body>

[<Scenario(Caption="Undo auto-correct MarkDown headlines")>]
module ``Given a headline`` =
    open System.Windows.Markup

    [<When>]
    let ``<When> BACKSPACE is pressed within headline <Then> headline is NOT removed`` () =
        let doc = new FlowDocument(new Paragraph(new Headline("headline")))

        let result = (new MarkdownHeadline()).TryUndo(doc.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) 
        result.Success |> should be False

        // try more in the middle
        let result = (new MarkdownHeadline()).TryUndo(doc.ContentStart.GetPositionAtOffset(4)) 
        result.Success |> should be False

        doc |> headline |> should not' (equal None)

    [<When>]
    let ``<When> last content of headline was removed <Then> headline is decoded into # characters again`` () =
        let doc = new FlowDocument(new Paragraph(new Headline("headline")))

        let h = doc |> headline |> Option.get
        h.Text <- ""

        let result = (new MarkdownHeadline()).TryUndo(h.ContentStart) 
        
        result.Success |> should be True

        doc |> headline |> should equal None
        doc |> text |> should equal "##\r\n"

    [<When>]
    let ``<When> headline was serialized <Then> level can still be reconstructed`` () =
        let doc = new FlowDocument(new Paragraph(new Run("### small")))

        doc 
        |> simulateReturn
        |> (new MarkdownHeadline()).TryApply
        |> success
        |> should be True

        XamlReader.Parse(XamlWriter.Save(doc)) :?> FlowDocument
        |> headline
        |> function
            | Some h -> h.Level |> should equal 3
            | None -> Assert.Fail("No headline found")

