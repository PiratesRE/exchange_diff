using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To8000))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To195))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To128))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength8))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To102400))]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To4000))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To32000))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To12000))]
	[Serializable]
	public abstract class DirectoryPropertyBinarySingle : DirectoryPropertyBinary
	{
	}
}
