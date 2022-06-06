using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsPrefix( "http://github.com/ronin4net/plainion", "pn" )]
[assembly: XmlnsDefinition( "http://github.com/ronin4net/plainion", "Plainion.Windows" )]
[assembly: XmlnsDefinition( "http://github.com/ronin4net/plainion", "Plainion.Windows.Controls" )]
[assembly: XmlnsDefinition( "http://github.com/ronin4net/plainion", "Plainion.Windows.Controls.Tree" )]
[assembly: XmlnsDefinition( "http://github.com/ronin4net/plainion", "Plainion.Windows.Controls.Text" )]
[assembly: XmlnsDefinition( "http://github.com/ronin4net/plainion", "Plainion.Windows.Interactivity" )]
[assembly: XmlnsDefinition( "http://github.com/ronin4net/plainion", "Plainion.Windows.Interactivity.DragDrop" )]

// sn.exe -Tp <assembly>
[assembly: InternalsVisibleTo("Plainion.Windows.Tests")]
[assembly: InternalsVisibleTo("Plainion.Windows.Specs")]

[assembly: SupportedOSPlatform("windows7.0")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]