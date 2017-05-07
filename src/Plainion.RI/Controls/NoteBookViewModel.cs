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
            fs.File( "/x/a/1" ).Create();
            fs.File( "/x/b/2/x" ).Create();
            fs.File( "/x/b/2/y" ).Create();

            DocumentStore = new FileSystemDocumentStore( fs.Directory( "/x" ) );
        }

        public IDocumentStore DocumentStore { get; private set; }
    }
}
