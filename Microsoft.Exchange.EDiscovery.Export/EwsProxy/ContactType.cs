using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class ContactType : EntityType
	{
		public string PersonName
		{
			get
			{
				return this.personNameField;
			}
			set
			{
				this.personNameField = value;
			}
		}

		public string BusinessName
		{
			get
			{
				return this.businessNameField;
			}
			set
			{
				this.businessNameField = value;
			}
		}

		[XmlArrayItem("Phone", IsNullable = false)]
		public PhoneType[] PhoneNumbers
		{
			get
			{
				return this.phoneNumbersField;
			}
			set
			{
				this.phoneNumbersField = value;
			}
		}

		[XmlArrayItem("Url", IsNullable = false)]
		public string[] Urls
		{
			get
			{
				return this.urlsField;
			}
			set
			{
				this.urlsField = value;
			}
		}

		[XmlArrayItem("EmailAddress", IsNullable = false)]
		public string[] EmailAddresses
		{
			get
			{
				return this.emailAddressesField;
			}
			set
			{
				this.emailAddressesField = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public string[] Addresses
		{
			get
			{
				return this.addressesField;
			}
			set
			{
				this.addressesField = value;
			}
		}

		public string ContactString
		{
			get
			{
				return this.contactStringField;
			}
			set
			{
				this.contactStringField = value;
			}
		}

		private string personNameField;

		private string businessNameField;

		private PhoneType[] phoneNumbersField;

		private string[] urlsField;

		private string[] emailAddressesField;

		private string[] addressesField;

		private string contactStringField;
	}
}
