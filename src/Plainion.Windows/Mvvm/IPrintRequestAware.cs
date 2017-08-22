using System.Printing;
using System.Windows;
using System.Windows.Documents;

namespace Plainion.Windows.Mvvm
{
    public interface IPrintRequestAware
    {
        DocumentPaginator GetPaginator( Size printSize );
        PageOrientation PreferredOrientation { get; }
    }
}
