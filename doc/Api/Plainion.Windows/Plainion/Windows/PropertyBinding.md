
# Plainion.Windows.PropertyBinding

**Namespace:** Plainion.Windows

**Assembly:** Plainion.Windows


## Methods

### void Bind(System.Linq.Expressions.Expression`1[System.Func`1[T]] source,System.Linq.Expressions.Expression`1[System.Func`1[T]] target)

### void Bind(System.Linq.Expressions.Expression`1[System.Func`1[T]] source,System.Linq.Expressions.Expression`1[System.Func`1[T]] target,System.Windows.Data.BindingMode mode)

### void Bind(Plainion.Windows.BindableProperty source,Plainion.Windows.BindableProperty target,System.Windows.Data.BindingMode mode)

Binds two properties where both declaring types implement INotifyPropertyChanged. Supported BindingModes: OneWay, OneWayToSource, TwoWay

### void Observe(System.Linq.Expressions.Expression`1[System.Func`1[T]] source,System.EventHandler`1[System.ComponentModel.PropertyChangedEventArgs] handler)
