using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    // TODO: add documents via path "/a/b/c[2]"
    public interface IDocumentStore
    {
        Document Get(DocumentId id);

        void Save(Document document);

        void Delete(DocumentId id);

        Folder Root { get; }

        IReadOnlyCollection<Document> Search(string text);
    }

    public static class DocumentStore
    {
        public static Folder FindFolder(this IDocumentStore self, DocumentId id)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNull(id, "id");

            return self.Root.FindFolder(id);
        }

        public static Folder FindFolder(this Folder folder, DocumentId id)
        {
            if(folder.Documents.Any(x => x == id))
            {
                return folder;
            }

            foreach(var child in folder.Children)
            {
                var found = child.FindFolder(id);
                if(found != null)
                {
                    return found;
                }
            }
            return null;
        }
    }
}
