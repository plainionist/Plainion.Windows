using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    public class DocumentPath
    {
        public DocumentPath(IEnumerable<string> path)
        {
            Contract.RequiresNotNullNotEmpty(path, "path");
            Contract.Requires(path.All(p => !p.Contains("/", StringComparison.OrdinalIgnoreCase)), "Invalid character in path: '/'");

            Paths = path.ToList();
            Name = Paths.First();
            AsPath = "/" + string.Join("/", Paths);
        }

        public IReadOnlyCollection<string> Paths { get; private set; }

        public string Name { get; private set; }

        public string AsPath { get; private set; }

        /// <summary>
        /// Path separated by '/'
        /// </summary>
        public static DocumentPath Parse(string path)
        {
            return new DocumentPath(path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
