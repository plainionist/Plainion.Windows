
# Plainion.Windows.PropertyBinding

**Namespace:** Plainion.Windows

**Assembly:** Plainion.Windows


## Methods

### Plainion.Windows.BindingId Bind(System.Linq.Expressions.Expression`1[System.Func`1[T]] source,System.Linq.Expressions.Expression`1[System.Func`1[T]] target)

### Plainion.Windows.BindingId Bind(System.Linq.Expressions.Expression`1[System.Func`1[T]] source,System.Linq.Expressions.Expression`1[System.Func`1[T]] target,System.Windows.Data.BindingMode mode)

### Plainion.Windows.BindingId Bind(Plainion.Windows.BindableProperty source,Plainion.Windows.BindableProperty target,System.Windows.Data.BindingMode mode)

Binds two properties where both declaring types implement INotifyPropertyChanged. Supported BindingModes: OneWay, OneWayToSource, TwoWay

#### Return value

The id of the binding which can be used to unbind source and target.

### Plainion.Windows.BindingId Observe(System.Linq.Expressions.Expression`1[System.Func`1[T]] source,System.EventHandler`1[System.ComponentModel.PropertyChangedEventArgs] handler)

### void Unbind(Plainion.Windows.BindingId id)

Removes the binding specified by the given id.
