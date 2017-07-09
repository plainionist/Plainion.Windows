using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    class Snapshot
    {
        public Snapshot(Folder root)
        {
            var folders = root.Enumerate().ToList();

            Folders = folders.Select(f => f.Id).ToList();
            Documents = folders
                .SelectMany(f => f.Documents)
                .Select(doc=>doc.Id)
                .ToList();
        }

        public IReadOnlyCollection<FolderId> Folders { get; private set; }

        public IReadOnlyCollection<DocumentId> Documents { get; private set; }
    }
}
