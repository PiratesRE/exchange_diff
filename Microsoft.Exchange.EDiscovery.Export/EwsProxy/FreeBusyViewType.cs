using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Flags]
	[Serializable]
	public enum FreeBusyViewType
	{
		None = 1,
		MergedOnly = 2,
		FreeBusy = 4,
		FreeBusyMerged = 8,
		Detailed = 16,
		DetailedMerged = 32
	}
}
