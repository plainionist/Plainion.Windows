using System.ComponentModel;

namespace Plainion.Windows.Controls.Text
{
    public interface IStoreItem : INotifyPropertyChanged
    {
        string Title { get; set; }
    }
}
