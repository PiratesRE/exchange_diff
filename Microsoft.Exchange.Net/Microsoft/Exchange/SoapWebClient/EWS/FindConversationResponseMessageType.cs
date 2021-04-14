using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class FindConversationResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Conversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ConversationType[] Conversations;

		[XmlArrayItem("Term", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public HighlightTermType[] HighlightTerms;

		public int TotalConversationsInView;

		[XmlIgnore]
		public bool TotalConversationsInViewSpecified;

		public int IndexedOffset;

		[XmlIgnore]
		public bool IndexedOffsetSpecified;
	}
}
