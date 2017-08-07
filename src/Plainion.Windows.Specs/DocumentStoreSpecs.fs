namespace Plainion.Windows.Specs.Controls.Text

open System.Threading
open NUnit.Framework
open Plainion.Windows.Controls.Text
open Plainion.Windows.Specs.Controls.Text
open System.IO
open Plainion.IO.MemoryFS

[<Apartment(ApartmentState.STA)>]
[<Spec>]
module ``Given a DocumentStore`` =

    let create path =
        let fs = new FileSystemImpl()
        let store = new FileSystemDocumentStore(fs.Directory(path))
        store.Initialize()
        fs,store

    [<Test>]
    let ``<When> adding documents <And> triggering save <Then> the new documents are saved``() =
        let fs,store = create "/x"
        store.Root.Entries.Add(new Document(fun () -> newDocument "test"))

        store.Root.IsModified |> succeeded

        store.SaveChanges()
        // TODO: check through serialization
        // index file + meta and body file for one document
        Assert.That(fs.Directory("/x").EnumerateFiles("*", SearchOption.AllDirectories) |> Seq.length, Is.EqualTo(3))

        store.Create("/sub/doc2").Body |> addText "test2"

        store.Root.IsModified |> succeeded

        store.SaveChanges()

        // index file + meta and body file for two documents
        Assert.That(fs.Directory("/x").EnumerateFiles("*", SearchOption.AllDirectories) |> Seq.length, Is.EqualTo(5))


