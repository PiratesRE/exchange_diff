using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class AddNewTelUriContactToGroupType : BaseRequestType
	{
		public string TelUriAddress
		{
			get
			{
				return this.telUriAddressField;
			}
			set
			{
				this.telUriAddressField = value;
			}
		}

		public string ImContactSipUriAddress
		{
			get
			{
				return this.imContactSipUriAddressField;
			}
			set
			{
				this.imContactSipUriAddressField = value;
			}
		}

		public string ImTelephoneNumber
		{
			get
			{
				return this.imTelephoneNumberField;
			}
			set
			{
				this.imTelephoneNumberField = value;
			}
		}

		public ItemIdType GroupId
		{
			get
			{
				return this.groupIdField;
			}
			set
			{
				this.groupIdField = value;
			}
		}

		private string telUriAddressField;

		private string imContactSipUriAddressField;

		private string imTelephoneNumberField;

		private ItemIdType groupIdField;
	}
}
