using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PostItemType : ItemType
	{
		[XmlElement(DataType = "base64Binary")]
		public byte[] ConversationIndex
		{
			get
			{
				return this.conversationIndexField;
			}
			set
			{
				this.conversationIndexField = value;
			}
		}

		public string ConversationTopic
		{
			get
			{
				return this.conversationTopicField;
			}
			set
			{
				this.conversationTopicField = value;
			}
		}

		public SingleRecipientType From
		{
			get
			{
				return this.fromField;
			}
			set
			{
				this.fromField = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.internetMessageIdField;
			}
			set
			{
				this.internetMessageIdField = value;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.isReadField;
			}
			set
			{
				this.isReadField = value;
			}
		}

		[XmlIgnore]
		public bool IsReadSpecified
		{
			get
			{
				return this.isReadFieldSpecified;
			}
			set
			{
				this.isReadFieldSpecified = value;
			}
		}

		public DateTime PostedTime
		{
			get
			{
				return this.postedTimeField;
			}
			set
			{
				this.postedTimeField = value;
			}
		}

		[XmlIgnore]
		public bool PostedTimeSpecified
		{
			get
			{
				return this.postedTimeFieldSpecified;
			}
			set
			{
				this.postedTimeFieldSpecified = value;
			}
		}

		public string References
		{
			get
			{
				return this.referencesField;
			}
			set
			{
				this.referencesField = value;
			}
		}

		public SingleRecipientType Sender
		{
			get
			{
				return this.senderField;
			}
			set
			{
				this.senderField = value;
			}
		}

		private byte[] conversationIndexField;

		private string conversationTopicField;

		private SingleRecipientType fromField;

		private string internetMessageIdField;

		private bool isReadField;

		private bool isReadFieldSpecified;

		private DateTime postedTimeField;

		private bool postedTimeFieldSpecified;

		private string referencesField;

		private SingleRecipientType senderField;
	}
}
