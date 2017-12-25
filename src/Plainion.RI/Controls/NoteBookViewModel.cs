using System.ComponentModel.Composition;
using System.Windows.Documents;
using Plainion.Windows.Controls.Text;
using Plainion.Windows.Controls.Text.AutoCorrection;
using Prism.Commands;
using Prism.Mvvm;

namespace Plainion.RI.Controls
{
    [Export]
    class NoteBookViewModel : BindableBase
    {
        private const string RootPath = "c:/temp/fs";

        private FileSystemDocumentStore myDocumentStore;

        public NoteBookViewModel()
        {
            AutoCorrection = new AutoCorrectionTable();
            AutoCorrection.Corrections.Add(new SampleCorrection());

            var fs = new Plainion.IO.MemoryFS.FileSystemImpl();
            var root = fs.Directory(RootPath);
            if(root.Exists)
            {
                root.Delete(true);
            }
            root.Create();

            DocumentStore = new FileSystemDocumentStore(root);
            DocumentStore.Initialize();

            DocumentStore.Create("/User documentation/Installation").Body.AddText("Installation");
            DocumentStore.Create("/User documentation/Getting started").Body.AddText("Getting started");
            DocumentStore.Create("/User documentation/FAQ").Body.AddText("Frequenty Asked Questions");
            DocumentStore.Create("/Developer documentation/Getting started").Body.AddText("Getting started as a developer");
            DocumentStore.Create("/Developer documentation/HowTos/MVC with F#").Body.AddText("MVC with F#");
            DocumentStore.Create("/Developer documentation/HowTos/WebApi with F#").Body.AddText("WebApi with F#");

            SaveCommand = new DelegateCommand(() =>
            {
                DocumentStore.SaveChanges();

                var store = new FileSystemDocumentStore(fs.Directory(RootPath));
                store.Initialize();

                DocumentStore = store;
            });
        }

        public FileSystemDocumentStore DocumentStore
        {
            get { return myDocumentStore; }
            private set { SetProperty(ref myDocumentStore, value); }
        }

        public DelegateCommand SaveCommand { get; private set; }

        public AutoCorrectionTable AutoCorrection { get; private set; }
    }

    static class FlowDocumentExtensions
    {
        public static FlowDocument AddText(this FlowDocument self, string text)
        {
            self.Blocks.Add(new Paragraph(new Run(text)));
            return self;
        }
    }
}

