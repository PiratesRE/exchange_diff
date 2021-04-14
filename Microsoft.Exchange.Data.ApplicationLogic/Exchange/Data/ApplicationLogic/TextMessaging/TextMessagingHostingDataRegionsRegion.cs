using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[Serializable]
	public class TextMessagingHostingDataRegionsRegion
	{
		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public string CountryCode
		{
			get
			{
				return this.countryCodeField;
			}
			set
			{
				this.countryCodeField = value;
			}
		}

		[XmlElement(Form = XmlSchemaForm.Unqualified)]
		public string PhoneNumberExample
		{
			get
			{
				return this.phoneNumberExampleField;
			}
			set
			{
				this.phoneNumberExampleField = value;
			}
		}

		[XmlAttribute]
		public string Iso2
		{
			get
			{
				return this.iso2Field;
			}
			set
			{
				this.iso2Field = value;
			}
		}

		private string countryCodeField;

		private string phoneNumberExampleField;

		private string iso2Field;
	}
}
