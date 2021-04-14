using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindConversationResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Conversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ConversationType[] Conversations
		{
			get
			{
				return this.conversationsField;
			}
			set
			{
				this.conversationsField = value;
			}
		}

		[XmlArrayItem("Term", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public HighlightTermType[] HighlightTerms
		{
			get
			{
				return this.highlightTermsField;
			}
			set
			{
				this.highlightTermsField = value;
			}
		}

		public int TotalConversationsInView
		{
			get
			{
				return this.totalConversationsInViewField;
			}
			set
			{
				this.totalConversationsInViewField = value;
			}
		}

		[XmlIgnore]
		public bool TotalConversationsInViewSpecified
		{
			get
			{
				return this.totalConversationsInViewFieldSpecified;
			}
			set
			{
				this.totalConversationsInViewFieldSpecified = value;
			}
		}

		public int IndexedOffset
		{
			get
			{
				return this.indexedOffsetField;
			}
			set
			{
				this.indexedOffsetField = value;
			}
		}

		[XmlIgnore]
		public bool IndexedOffsetSpecified
		{
			get
			{
				return this.indexedOffsetFieldSpecified;
			}
			set
			{
				this.indexedOffsetFieldSpecified = value;
			}
		}

		private ConversationType[] conversationsField;

		private HighlightTermType[] highlightTermsField;

		private int totalConversationsInViewField;

		private bool totalConversationsInViewFieldSpecified;

		private int indexedOffsetField;

		private bool indexedOffsetFieldSpecified;
	}
}
