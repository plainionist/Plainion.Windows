namespace Plainion.Windows.Text

open System.Linq
open System.Threading
open NUnit.Framework
open FsUnit
open Plainion.IO
open Plainion.Windows.Controls.Text

[<Feature>]
module ``Creating documents`` =

    [<Scenario>]
    module ``Given a Document`` =
        open System.Windows.Documents
        open System

        [<When>]
        let ``<When> newly created <Then> IsModified is true``() =
            let doc = new Document(fun () -> new FlowDocument())

            doc.IsModified |> should be True

        [<When>]
        let ``<When> restored from store and body is not instantiated <Then> IsModified is false``() =
            let meta = new StoreItemMetaInfo<DocumentId>(new DocumentId(Guid.NewGuid()), DateTime.UtcNow, DateTime.UtcNow);
            let doc = new Document(meta, fun () -> new FlowDocument())

            doc.IsModified |> should be False

[<Feature>]
module ``Storing documents`` =

    [<Apartment(ApartmentState.STA)>]
    [<Scenario>]
    module ``Given any DocumentStore`` =
        let create (fs:IFileSystem) = 
            let store = new FileSystemDocumentStore(fs.Directory("/x"))
            store.Initialize()
            store

        [<When>]
        let ``<When> adding documents <And> triggering save <Then> the new documents are saved``() =
            let fs = new Plainion.IO.MemoryFS.FileSystemImpl()
            let store = create fs

            let doc = store.Create("/doc1")
            doc.Body |> addText "test1"

            doc.IsModified |> should be True
            store.Root.IsModified |> should be True

            store.SaveChanges()
            let store = create fs

            store.Root.IsModified |> should be False
            store.Get("/doc1") |> should not' (be Null)

            store.Create("/sub/doc2").Body |> addText "test2"

            store.SaveChanges()
            let store = create fs

            store.Get("/doc1") |> should not' (be Null)
            store.Get("/sub/doc2") |> should not' (be Null)

        [<When>]
        let ``<When> changing documents <And> triggering save <Then> the modified documents are saved``() =
            let fs = new Plainion.IO.MemoryFS.FileSystemImpl()
            let store = create fs

            store.Create("/doc1").Body |> addText "test1"
            store.Create("/sub/doc2").Body |> addText "test2"

            store.SaveChanges()
            let store = create fs

            store.Get("/doc1").Body |> addText "MORE"
            store.Get("/sub/doc2").Body |> addText "MUCH-MORE"
        
            store.SaveChanges()
            let store = create fs

            store.Get("/doc1").Body |> text |> should haveSubstring "MORE"
            store.Get("/sub/doc2").Body |> text |> should haveSubstring "MUCH-MORE"
        
        [<When>]
        let ``<When> removing documents <And> triggering save <Then> the removal is made persistent``() =
            let fs = new Plainion.IO.MemoryFS.FileSystemImpl()
            let store = create fs

            store.Create("/doc1").Body |> addText "test1"
            store.Create("/sub/doc2").Body |> addText "test2"

            store.SaveChanges()
            let store = create fs

            store.Root.Entries.Remove(store.Get("/doc1")) |> ignore
            store.Root.IsModified |> should be True
        
            store.SaveChanges()
            let store = create fs

            store.Root.Entries.OfType<Document>() |> List.ofSeq |> should haveLength 0
        
    [<Apartment(ApartmentState.STA)>]
    [<Scenario>]
    module ``Given a DocumentStore from previous version`` =
        open System.Windows.Documents
        open System.IO

        [<When>]
        let ``<When> was created with version 1 <Then> it will be converted during initialization``() =
            let location = typeof<ScenarioAttribute>.Assembly.Location |> Path.GetDirectoryName
            let file = Path.Combine(location, "TestData", "FileSystemDocumentStore.v1")

            let fs = 
                use stream = new FileStream(file,FileMode.Open, FileAccess.Read)
                Plainion.IO.MemoryFS.FileSystemImpl.Deserialize(stream)

            let store = new FileSystemDocumentStore(fs.Directory(@"C:\x"))
            store.Initialize()

            let text (doc:Document) = doc.Body.Content().Text

            store.Get("/User documentation/Installation") |> text |> should equal "Installation\r\n"
            store.Get("/User documentation/Getting started") |> text |> should equal "Getting started\r\n"
            store.Get("/Developer documentation/HowTos/MVC with F#") |> text |> should equal "MVC with F#\r\n"
            store.Get("/Developer documentation/HowTos/WebApi with F#") |> text |> should equal "WebApi with F#\r\n"

        [<When>]
        let ``<When> was created with version 2 <Then> it will be converted during initialization``() =
            let location = typeof<ScenarioAttribute>.Assembly.Location |> Path.GetDirectoryName
            let file = Path.Combine(location, "TestData", "FileSystemDocumentStore.v2")

            let fs = 
                use stream = new FileStream(file,FileMode.Open, FileAccess.Read)
                Plainion.IO.MemoryFS.FileSystemImpl.Deserialize(stream)

            let store = new FileSystemDocumentStore(fs.Directory(@"C:\x"))
            store.Initialize()

            let text (doc:Document) = doc.Body.Content().Text

            store.Get("/User documentation/Installation") |> text |> should equal "Installation\r\n"
            store.Get("/User documentation/Getting started") |> text |> should equal "Getting started\r\n"
            store.Get("/Developer documentation/HowTos/MVC with F#") |> text |> should equal "MVC with F#\r\n"
            store.Get("/Developer documentation/HowTos/WebApi with F#") |> text |> should equal "WebApi with F#\r\n"

