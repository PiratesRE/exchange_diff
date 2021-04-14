using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum SearchItemKindType
	{
		Email,
		Meetings,
		Tasks,
		Notes,
		Docs,
		Journals,
		Contacts,
		Im,
		Voicemail,
		Faxes,
		Posts,
		Rssfeeds
	}
}
