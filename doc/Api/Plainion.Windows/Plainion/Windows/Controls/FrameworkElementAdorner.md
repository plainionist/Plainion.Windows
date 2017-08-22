
# Plainion.Windows.Controls.FrameworkElementAdorner

**Namespace:** Plainion.Windows.Controls

**Assembly:** Plainion.Windows

This class is an adorner that allows a FrameworkElement derived class to adorn another FrameworkElement.

## Remarks

Initial version taken from: http://www.codeproject.com/Articles/57984/WPF-Loading-Wait-Adorner which was inspired by: http://www.codeproject.com/KB/WPF/WPFJoshSmith.aspx


## Constructors

### Constructor(System.Windows.FrameworkElement adornerChildElement,System.Windows.FrameworkElement adornedElement)

### Constructor(System.Windows.FrameworkElement adornerChildElement,System.Windows.FrameworkElement adornedElement,Plainion.Windows.Controls.AdornerPlacement horizontalAdornerPlacement,Plainion.Windows.Controls.AdornerPlacement verticalAdornerPlacement,System.Double offsetX,System.Double offsetY)


## Properties

### System.Double PositionX

### System.Double PositionY

### System.Int32 VisualChildrenCount

### System.Collections.IEnumerator LogicalChildren

### System.Windows.FrameworkElement AdornedElement


## Methods

### System.Windows.Size MeasureOverride(System.Windows.Size constraint)

### System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)

### System.Windows.Media.Visual GetVisualChild(System.Int32 index)

### void DisconnectChild()
