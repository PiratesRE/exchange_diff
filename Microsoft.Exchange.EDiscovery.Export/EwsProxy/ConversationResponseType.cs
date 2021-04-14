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
	[Serializable]
	public class ConversationResponseType
	{
		public ItemIdType ConversationId
		{
			get
			{
				return this.conversationIdField;
			}
			set
			{
				this.conversationIdField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] SyncState
		{
			get
			{
				return this.syncStateField;
			}
			set
			{
				this.syncStateField = value;
			}
		}

		[XmlArrayItem("ConversationNode", IsNullable = false)]
		public ConversationNodeType[] ConversationNodes
		{
			get
			{
				return this.conversationNodesField;
			}
			set
			{
				this.conversationNodesField = value;
			}
		}

		private ItemIdType conversationIdField;

		private byte[] syncStateField;

		private ConversationNodeType[] conversationNodesField;
	}
}
