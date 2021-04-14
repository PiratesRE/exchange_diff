using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class MailTips
	{
		public EmailAddressType RecipientAddress
		{
			get
			{
				return this.recipientAddressField;
			}
			set
			{
				this.recipientAddressField = value;
			}
		}

		public MailTipTypes PendingMailTips
		{
			get
			{
				return this.pendingMailTipsField;
			}
			set
			{
				this.pendingMailTipsField = value;
			}
		}

		public OutOfOfficeMailTip OutOfOffice
		{
			get
			{
				return this.outOfOfficeField;
			}
			set
			{
				this.outOfOfficeField = value;
			}
		}

		public bool MailboxFull
		{
			get
			{
				return this.mailboxFullField;
			}
			set
			{
				this.mailboxFullField = value;
			}
		}

		[XmlIgnore]
		public bool MailboxFullSpecified
		{
			get
			{
				return this.mailboxFullFieldSpecified;
			}
			set
			{
				this.mailboxFullFieldSpecified = value;
			}
		}

		public string CustomMailTip
		{
			get
			{
				return this.customMailTipField;
			}
			set
			{
				this.customMailTipField = value;
			}
		}

		public int TotalMemberCount
		{
			get
			{
				return this.totalMemberCountField;
			}
			set
			{
				this.totalMemberCountField = value;
			}
		}

		[XmlIgnore]
		public bool TotalMemberCountSpecified
		{
			get
			{
				return this.totalMemberCountFieldSpecified;
			}
			set
			{
				this.totalMemberCountFieldSpecified = value;
			}
		}

		public int ExternalMemberCount
		{
			get
			{
				return this.externalMemberCountField;
			}
			set
			{
				this.externalMemberCountField = value;
			}
		}

		[XmlIgnore]
		public bool ExternalMemberCountSpecified
		{
			get
			{
				return this.externalMemberCountFieldSpecified;
			}
			set
			{
				this.externalMemberCountFieldSpecified = value;
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return this.maxMessageSizeField;
			}
			set
			{
				this.maxMessageSizeField = value;
			}
		}

		[XmlIgnore]
		public bool MaxMessageSizeSpecified
		{
			get
			{
				return this.maxMessageSizeFieldSpecified;
			}
			set
			{
				this.maxMessageSizeFieldSpecified = value;
			}
		}

		public bool DeliveryRestricted
		{
			get
			{
				return this.deliveryRestrictedField;
			}
			set
			{
				this.deliveryRestrictedField = value;
			}
		}

		[XmlIgnore]
		public bool DeliveryRestrictedSpecified
		{
			get
			{
				return this.deliveryRestrictedFieldSpecified;
			}
			set
			{
				this.deliveryRestrictedFieldSpecified = value;
			}
		}

		public bool IsModerated
		{
			get
			{
				return this.isModeratedField;
			}
			set
			{
				this.isModeratedField = value;
			}
		}

		[XmlIgnore]
		public bool IsModeratedSpecified
		{
			get
			{
				return this.isModeratedFieldSpecified;
			}
			set
			{
				this.isModeratedFieldSpecified = value;
			}
		}

		public bool InvalidRecipient
		{
			get
			{
				return this.invalidRecipientField;
			}
			set
			{
				this.invalidRecipientField = value;
			}
		}

		[XmlIgnore]
		public bool InvalidRecipientSpecified
		{
			get
			{
				return this.invalidRecipientFieldSpecified;
			}
			set
			{
				this.invalidRecipientFieldSpecified = value;
			}
		}

		public int Scope
		{
			get
			{
				return this.scopeField;
			}
			set
			{
				this.scopeField = value;
			}
		}

		[XmlIgnore]
		public bool ScopeSpecified
		{
			get
			{
				return this.scopeFieldSpecified;
			}
			set
			{
				this.scopeFieldSpecified = value;
			}
		}

		private EmailAddressType recipientAddressField;

		private MailTipTypes pendingMailTipsField;

		private OutOfOfficeMailTip outOfOfficeField;

		private bool mailboxFullField;

		private bool mailboxFullFieldSpecified;

		private string customMailTipField;

		private int totalMemberCountField;

		private bool totalMemberCountFieldSpecified;

		private int externalMemberCountField;

		private bool externalMemberCountFieldSpecified;

		private int maxMessageSizeField;

		private bool maxMessageSizeFieldSpecified;

		private bool deliveryRestrictedField;

		private bool deliveryRestrictedFieldSpecified;

		private bool isModeratedField;

		private bool isModeratedFieldSpecified;

		private bool invalidRecipientField;

		private bool invalidRecipientFieldSpecified;

		private int scopeField;

		private bool scopeFieldSpecified;
	}
}
