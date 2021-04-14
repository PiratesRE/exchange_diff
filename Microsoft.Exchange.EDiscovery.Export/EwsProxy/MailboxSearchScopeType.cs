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
	public class MailboxSearchScopeType
	{
		public string Mailbox
		{
			get
			{
				return this.mailboxField;
			}
			set
			{
				this.mailboxField = value;
			}
		}

		public MailboxSearchLocationType SearchScope
		{
			get
			{
				return this.searchScopeField;
			}
			set
			{
				this.searchScopeField = value;
			}
		}

		[XmlArrayItem("ExtendedAttribute", IsNullable = false)]
		public ExtendedAttributeType[] ExtendedAttributes
		{
			get
			{
				return this.extendedAttributesField;
			}
			set
			{
				this.extendedAttributesField = value;
			}
		}

		private string mailboxField;

		private MailboxSearchLocationType searchScopeField;

		private ExtendedAttributeType[] extendedAttributesField;
	}
}
