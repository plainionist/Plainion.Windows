using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public static class DocumentStoreExtensions
    {
        /// <summary>
        /// Returns the folder containing the given document.
        /// </summary>
        public static Folder Folder(this DocumentStore self, DocumentId id)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNull(id, "id");

            return self.Root.Folder(id);
        }

        /// <summary>
        /// Returns the folder containing the given document.
        /// </summary>
        public static Folder Folder(this Folder self, DocumentId id)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNull(id, "id");

            if (self.Documents.Any(x => x == id))
            {
                return self;
            }

            foreach (var child in self.Children)
            {
                var found = child.Folder(id);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static IEnumerable<Folder> Enumerate(this Folder self)
        {
            Contract.RequiresNotNull(self, "self");

            yield return self;

            foreach (var child in self.Children.SelectMany(c => c.Enumerate()))
            {
                yield return child;
            }
        }

        /// <summary>
        /// Creates a new document at the given path. Path elements are separated with "/".
        /// The last path element is the title of the document.
        /// </summary>
        public static Document Create(this DocumentStore self, string path)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNullNotEmpty(path, "path");

            var elements = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Contract.Requires(elements.Length > 0, "Path is pointing to root. Not a valid document path.");

            var folder = self.Root;
            foreach (var element in elements.Take(elements.Length - 1))
            {
                var child = folder.Children.FirstOrDefault(f => f.Title.Equals(element, StringComparison.OrdinalIgnoreCase));
                if (child == null)
                {
                    child = new Folder();
                    folder.Children.Add(child);
                }

                folder = child;
            }

            var documentTitle = elements[elements.Length - 1];

            Contract.Invariant(!folder.Documents
                .Select(id => self.Get(id))
                .Any(doc => doc.Title.Equals(documentTitle, StringComparison.OrdinalIgnoreCase)), "Document already exists");

            var document = new Document(() => new FlowDocument());
            document.Title = documentTitle;

            folder.Documents.Add(document.Id);

            return document;
        }

        internal static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }
    }
}
