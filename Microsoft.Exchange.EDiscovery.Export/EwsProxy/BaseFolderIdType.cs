using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlInclude(typeof(AddressListIdType))]
	[XmlInclude(typeof(FolderIdType))]
	[XmlInclude(typeof(DistinguishedFolderIdType))]
	[Serializable]
	public abstract class BaseFolderIdType
	{
	}
}
