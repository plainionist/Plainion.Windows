﻿using System.Windows.Markup;

namespace Plainion.Windows.Tests.Xaml
{
    [ContentProperty( "Content" )]
    public class Model
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public int Value { get; set; }
    }
}
