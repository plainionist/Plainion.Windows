using System.ComponentModel.Composition;
using Plainion.IO.MemoryFS;
using Plainion.Windows.Controls.Text;

namespace Plainion.RI.Controls
{
    [Export]
    class NoteBookViewModel
    {
        public NoteBookViewModel()
        {
            var fs = new FileSystemImpl();

            DocumentStore = new FileSystemDocumentStore(fs.Directory("/x"));

            DocumentStore.Create(DocumentPath.Parse("/User documentation/Installation"));
            DocumentStore.Create(DocumentPath.Parse("/User documentation/Getting started"));
            DocumentStore.Create(DocumentPath.Parse("/User documentation/FAQ"));
            DocumentStore.Create(DocumentPath.Parse("/Developer documentation/Getting started"));
            DocumentStore.Create(DocumentPath.Parse("/Developer documentation/HowTos/MVC with F#"));
            DocumentStore.Create(DocumentPath.Parse("/Developer documentation/HowTos/WebApi with F#"));
        }

        public IDocumentStore DocumentStore { get; private set; }
    }
}
