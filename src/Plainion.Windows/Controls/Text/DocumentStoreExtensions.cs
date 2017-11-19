using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

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

            if (self.Entries.OfType<Document>().Any(doc => doc.Id == id))
            {
                return self;
            }

            foreach (var child in self.Entries.OfType<Folder>())
            {
                var found = child.Folder(id);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static IEnumerable<IStoreItem> Enumerate(this Folder self)
        {
            Contract.RequiresNotNull(self, "self");

            yield return self;

            foreach (var child in self.Entries.OfType<Folder>().SelectMany(c => c.Enumerate()))
            {
                yield return child;
            }

            foreach (var child in self.Entries.OfType<Document>())
            {
                yield return child;
            }
        }

        /// <summary>
        /// Creates a new document with default settings.
        /// </summary>
        public static Document CreateDocument(string title)
        {
            var document = new Document(() => new FlowDocument()
            {
                FontFamily = TextStyles.Body.FontFamily,
                FontSize = TextStyles.Body.FontSize
            });
            document.Title = title;

            return document;
        }

        /// <summary>
        /// Creates a new document under the given folder with the given title
        /// </summary>
        public static Document Create(this DocumentStore self, Folder folder, string title)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNull(folder, "folder");

            var document = CreateDocument(title);

            folder.Entries.Add(document);

            return document;
        }

        /// <summary>
        /// Returns the document at the given path. Path elements are separated with "/".
        /// The last path element is the title of the document.
        /// </summary>
        /// <returns>the document if found, null otherwise</returns>
        public static Document TryGet(this DocumentStore self, string path)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNullNotEmpty(path, "path");

            var elements = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Contract.Requires(elements.Length > 0, "Path is pointing to root. Not a valid document path.");

            var folder = self.Root;
            foreach (var element in elements.Take(elements.Length - 1))
            {
                var child = folder.Entries.OfType<Folder>().FirstOrDefault(f => f.Title.Equals(element, StringComparison.OrdinalIgnoreCase));
                if (child == null)
                {
                    return null;
                }

                folder = child;
            }

            var title = elements[elements.Length - 1];
            var document = folder.Entries.OfType<Document>().SingleOrDefault(d => d.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (document == null)
            {
                return null;
            }

            return document;
        }

        /// <summary>
        /// Returns the document at the given path. Path elements are separated with "/".
        /// The last path element is the title of the document.
        /// </summary>
        /// <exception cref="ArgumentException">If there is no document at that path</exception>
        public static Document Get(this DocumentStore self, string path)
        {
            var document = TryGet(self, path);
            Contract.RequiresNotNull(document, "document not found at: " + path);

            return document;
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
                var child = folder.Entries.OfType<Folder>().FirstOrDefault(f => f.Title.Equals(element, StringComparison.OrdinalIgnoreCase));
                if (child == null)
                {
                    child = new Folder();
                    child.Title = element;
                    folder.Entries.Add(child);
                }

                folder = child;
            }

            var title = elements[elements.Length - 1];

            return Create(self, folder, title);
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
