using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AlternateMailbox
	{
		[XmlElement(IsNullable = true)]
		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string LegacyDN
		{
			get
			{
				return this.legacyDNField;
			}
			set
			{
				this.legacyDNField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string Server
		{
			get
			{
				return this.serverField;
			}
			set
			{
				this.serverField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string SmtpAddress
		{
			get
			{
				return this.smtpAddressField;
			}
			set
			{
				this.smtpAddressField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string OwnerSmtpAddress
		{
			get
			{
				return this.ownerSmtpAddressField;
			}
			set
			{
				this.ownerSmtpAddressField = value;
			}
		}

		private string typeField;

		private string displayNameField;

		private string legacyDNField;

		private string serverField;

		private string smtpAddressField;

		private string ownerSmtpAddressField;
	}
}
