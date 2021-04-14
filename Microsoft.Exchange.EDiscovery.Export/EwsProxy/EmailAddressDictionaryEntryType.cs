using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EmailAddressDictionaryEntryType
	{
		[XmlAttribute]
		public EmailAddressKeyType Key
		{
			get
			{
				return this.keyField;
			}
			set
			{
				this.keyField = value;
			}
		}

		[XmlAttribute]
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

		[XmlAttribute]
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

		[XmlAttribute]
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

		[XmlText]
		public string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private EmailAddressKeyType keyField;

		private string nameField;

		private string routingTypeField;

		private MailboxTypeType mailboxTypeField;

		private bool mailboxTypeFieldSpecified;

		private string valueField;
	}
}
