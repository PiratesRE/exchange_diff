using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetConversationItemsResponseMessageType : ResponseMessageType
	{
		public ConversationResponseType Conversation
		{
			get
			{
				return this.conversationField;
			}
			set
			{
				this.conversationField = value;
			}
		}

		private ConversationResponseType conversationField;
	}
}
