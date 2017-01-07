﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Plainion.Windows
{
    // Found here: http://stackoverflow.com/questions/2607490/is-there-a-way-to-chain-multiple-value-converters-in-xaml
    public class ConverterChain : List<IValueConverter>, IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return this.Aggregate( value, ( current, converter ) => converter.Convert( current, targetType, parameter, culture ) );
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
