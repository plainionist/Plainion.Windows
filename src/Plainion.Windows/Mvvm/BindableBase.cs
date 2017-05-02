using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plainion.Windows.Mvvm
{
    /// <summary>
    /// Simple implementation of INotifyPropertyChanged.
    /// If you need a more complete implementation pls use the one from Prism.
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if(object.Equals(storage, value))
            {
                return false;
            }

            storage = value;

            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
