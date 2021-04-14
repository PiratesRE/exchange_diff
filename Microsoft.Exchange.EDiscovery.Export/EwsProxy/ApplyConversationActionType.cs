using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class ApplyConversationActionType : BaseRequestType
	{
		[XmlArrayItem("ConversationAction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ConversationActionType[] ConversationActions
		{
			get
			{
				return this.conversationActionsField;
			}
			set
			{
				this.conversationActionsField = value;
			}
		}

		private ConversationActionType[] conversationActionsField;
	}
}
