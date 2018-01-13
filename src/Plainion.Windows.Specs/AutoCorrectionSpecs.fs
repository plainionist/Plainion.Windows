namespace Plainion.Windows.Specs.Controls.Text

open System.Windows.Documents
open Plainion.Windows.Controls.Text
open Plainion.Windows.Controls.Text.AutoCorrection
open NUnit.Framework
open FsUnit

[<Feature>]
module ``Auto-correct hyperlinks`` =
    let tryAutoCorrect (doc:FlowDocument) =
        (new ClickableHyperlink()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return))

    let getHyperlinks (doc:FlowDocument) =
        let visitor = new FlowDocumentVisitor(fun e -> e :? Hyperlink)
        visitor.Accept(doc)
        visitor.Results |> Seq.cast<Hyperlink> |> List.ofSeq

    [<Scenario>]
    module ``Given a text in URI format`` =
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

    [<Scenario>]
    module ``Given a Hyperlink`` =
        [<Test>]
        let ``<When> BACKSPACE is entered <Then> clickable hyperlink is removed`` () =
            let document = new FlowDocument(new Paragraph(new Run("Some dummy http://github.org/")));

            document |> tryAutoCorrect |> ignore
            let result = (new ClickableHyperlink()).TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) 
            result.Success |> should be True

            document |> getHyperlinks |> should be Empty


[<Feature>]
module ``Auto-correct Unicode symbols`` =
    let tryAutoCorrect (doc:FlowDocument) =
        (new UnicodeSymbolCorrection()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return))

    let text (doc:FlowDocument) = 
        let range = new TextRange(doc.ContentStart, doc.ContentEnd)
        range.Text

    [<Scenario>]
    module ``Given a known Ascii symbol`` =
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

            document |> text |> should haveSubstring "\u2794"

    [<Scenario>]
    module ``Given an unknown Ascii symbol`` =
        [<Test>]
        let ``<When> entered <Then> no symbol is inserted`` () =
            let document = new FlowDocument(new Paragraph(new Run("Some dummy text")));

            let result = document |> tryAutoCorrect 
        
            result.Success |> should be False

            document |> text |> should equal "Some dummy text\r\n"

    [<Scenario>]
    module ``Given a Unicode symbol`` =
        [<Test>]
        let ``<When> BACKSPACE is entered <Then> symbol is removed`` () =
            let document = new FlowDocument(new Paragraph(new Run("we conclude ==>")))

            document |> tryAutoCorrect |> ignore
            let result = (new UnicodeSymbolCorrection()).TryUndo(document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward))
            result.Success |> should be True

            document |> text |> should equal "we conclude ==>\r\n"


[<Feature>]
module ``Auto-correct MarkDown headlines`` =
    let headline (doc:FlowDocument) = 
        let visitor = new FlowDocumentVisitor(fun x -> x :? Headline)
        visitor.Accept(doc)
        visitor.Results |> Seq.cast<Headline> |> Seq.tryHead

    [<Scenario>]
    module ``Given a text line with leading # characters`` =
        [<Test>]
        let ``<When> SPACE is pressed after '#' character <Then> the line will be converted into a headline`` () =
            let doc = new FlowDocument(new Paragraph(new Run("# ")))

            let result = (new MarkdownHeadline()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Space))

            result.Success |> should be True

            let headline = doc |> headline 
            headline |> should not' (equal None)
            headline |> Option.map(fun x -> x.Text) |> should equal (Some(""))

        [<Test>]
        let ``<When> RETURN is pressed after a headline <Then> a new body text block is started`` () =
            let doc = new FlowDocument(new Paragraph(new Headline("headline")))

            let result = (new MarkdownHeadline()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return))
                    
            result.Success |> should be True

            doc.Blocks
            |> Seq.cast<Paragraph>
            |> Seq.last
            |> fun b -> b.Inlines
            |> Seq.last
            |> should be instanceOfType<Body>

    [<Scenario>]
    module ``Given a headline`` =
        open System.Windows.Markup

        [<Test>]
        let ``<When> BACKSPACE is pressed within headline <Then> headline is NOT removed`` () =
            let doc = new FlowDocument(new Paragraph(new Headline("headline")))

            let result = (new MarkdownHeadline()).TryUndo(doc.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward)) 
            result.Success |> should be False

            // try more in the middle
            let result = (new MarkdownHeadline()).TryUndo(doc.ContentStart.GetPositionAtOffset(4)) 
            result.Success |> should be False

            doc |> headline |> should not' (equal None)

        [<Test>]
        let ``<When> last content of headline was removed <Then> headline is decoded into # characters again`` () =
            let doc = new FlowDocument(new Paragraph(new Headline("headline")))

            let h = doc |> headline |> Option.get
            h.Text <- ""

            let result = (new MarkdownHeadline()).TryUndo(h.ContentStart) 
        
            result.Success |> should be True

            doc |> headline |> should equal None
            doc |> text |> should equal "##\r\n"

        [<Test>]
        let ``<When> headline was serialized <Then> level can still be reconstructed`` () =
            let doc = new FlowDocument(new Paragraph(new Run("### small")))

            let result = (new MarkdownHeadline()).TryApply(new AutoCorrectionInput(new TextRange(doc.ContentEnd, doc.ContentEnd), AutoCorrectionTrigger.Return))
            result.Success |> should be True

            XamlReader.Parse(XamlWriter.Save(doc)) :?> FlowDocument
            |> headline
            |> function
                | Some h -> h.Level |> should equal 3
                | None -> Assert.Fail("No headline found")

