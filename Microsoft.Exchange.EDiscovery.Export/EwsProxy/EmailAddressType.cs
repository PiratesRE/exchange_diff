using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EmailAddressType : BaseEmailAddressType
	{
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddressField;
			}
			set
			{
				this.emailAddressField = value;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingTypeField;
			}
			set
			{
				this.routingTypeField = value;
			}
		}

		public MailboxTypeType MailboxType
		{
			get
			{
				return this.mailboxTypeField;
			}
			set
			{
				this.mailboxTypeField = value;
			}
		}

		[XmlIgnore]
		public bool MailboxTypeSpecified
		{
			get
			{
				return this.mailboxTypeFieldSpecified;
			}
			set
			{
				this.mailboxTypeFieldSpecified = value;
			}
		}

		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		public string OriginalDisplayName
		{
			get
			{
				return this.originalDisplayNameField;
			}
			set
			{
				this.originalDisplayNameField = value;
			}
		}

		private string nameField;

		private string emailAddressField;

		private string routingTypeField;

		private MailboxTypeType mailboxTypeField;

		private bool mailboxTypeFieldSpecified;

		private ItemIdType itemIdField;

		private string originalDisplayNameField;
	}
}
