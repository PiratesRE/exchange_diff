using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IncludeInSchema = false)]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum ItemsChoiceType
	{
		CopiedEvent,
		CreatedEvent,
		DeletedEvent,
		FreeBusyChangedEvent,
		ModifiedEvent,
		MovedEvent,
		NewMailEvent,
		StatusEvent
	}
}
