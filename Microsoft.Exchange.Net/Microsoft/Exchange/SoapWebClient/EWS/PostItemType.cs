using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class PostItemType : ItemType
	{
		[XmlElement(DataType = "base64Binary")]
		public byte[] ConversationIndex;

		public string ConversationTopic;

		public SingleRecipientType From;

		public string InternetMessageId;

		public bool IsRead;

		[XmlIgnore]
		public bool IsReadSpecified;

		public DateTime PostedTime;

		[XmlIgnore]
		public bool PostedTimeSpecified;

		public string References;

		public SingleRecipientType Sender;
	}
}
