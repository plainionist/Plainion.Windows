
# Plainion.Windows.MultiStyleExtension

**Namespace:** Plainion.Windows

**Assembly:** Plainion.Windows

Markup extension to merge all Styles given via ResourceKeys into single Style instance.

## Remarks

Initial version taken from: http://web.archive.org/web/20101125040337/http://bea.stollnitz.com/blog/?p=384


## Constructors

### Constructor()


## Properties

### System.String ResourceKeys

Space separated list of resource keys


## Methods

### System.Object ProvideValue(System.IServiceProvider serviceProvider)

Returns a style that merges all styles with the keys specified by ResourceKeys property.
