
# Plainion.Windows.Mvvm.DelegateCommand`1

**Namespace:** Plainion.Windows.Mvvm

**Assembly:** Plainion.Windows

Simple "delegate" based command implementation. If you need a more complete implementation pls use the one from Prism.


## Constructors

### Constructor(System.Action`1[T] exec)

### Constructor(System.Action`1[T] exec,System.Func`2[T,System.Boolean] canExec)


## Events

### System.EventHandler CanExecuteChanged


## Methods

### System.Boolean CanExecute(System.Object parameter)

### void Execute(System.Object parameter)

### void RaiseCanExecuteChanged()
