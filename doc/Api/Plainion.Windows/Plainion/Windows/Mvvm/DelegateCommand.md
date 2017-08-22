
# Plainion.Windows.Mvvm.DelegateCommand

**Namespace:** Plainion.Windows.Mvvm

**Assembly:** Plainion.Windows

Simple "delegate" based command implementation. If you need a more complete implementation pls use the one from Prism.


## Constructors

### Constructor(System.Action execute)

### Constructor(System.Action exec,System.Func`1[System.Boolean] canExec)


## Events

### System.EventHandler CanExecuteChanged


## Methods

### System.Boolean CanExecute(System.Object parameter)

### void RaiseCanExecuteChanged()

### void Execute(System.Object parameter)
