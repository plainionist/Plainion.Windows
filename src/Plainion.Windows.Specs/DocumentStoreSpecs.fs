namespace Plainion.Windows.Specs.Controls.Text

open System.Threading
open NUnit.Framework
open Plainion.Windows.Controls.Text
open Plainion.Windows.Specs.Controls.Text
open System.IO
open Plainion.IO.MemoryFS
open FsUnit

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

        store.Root.IsModified |> should be True

        store.SaveChanges()
        // TODO: check through serialization
        // index file + meta and body file for one document
        fs.Directory("/x").EnumerateFiles("*", SearchOption.AllDirectories) |> List.ofSeq |> should haveLength 3

        store.Create("/sub/doc2").Body |> addText "test2"

        store.Root.IsModified |> should be True

        store.SaveChanges()

        // index file + meta and body file for two documents
        fs.Directory("/x").EnumerateFiles("*", SearchOption.AllDirectories) |> List.ofSeq |> should haveLength 5


