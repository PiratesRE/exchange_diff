using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ConnectingSIDType
	{
		[XmlElement]
		[DataMember(Order = 1)]
		public string PrincipalName
		{
			get
			{
				return this.principalNameField;
			}
			set
			{
				this.principalNameField = value;
			}
		}

		[DataMember(Order = 2)]
		[XmlElement]
		public string SID
		{
			get
			{
				return this.sIDField;
			}
			set
			{
				this.sIDField = value;
			}
		}

		[XmlElement]
		[DataMember(Order = 3)]
		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddressField;
			}
			set
			{
				this.primarySmtpAddressField = value;
			}
		}

		[XmlElement]
		[DataMember(Order = 4)]
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

		private string principalNameField;

		private string sIDField;

		private string primarySmtpAddressField;

		private string smtpAddressField;
	}
}
