using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Resolution")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "Resolution")]
	[Serializable]
	public class ResolutionType
	{
		[DataMember(Name = "Mailbox", EmitDefaultValue = false, Order = 1)]
		[XmlElement("Mailbox")]
		public EmailAddressWrapper Mailbox
		{
			get
			{
				return this.mailbox;
			}
			set
			{
				this.mailbox = value;
			}
		}

		[DataMember(Name = "Contact", EmitDefaultValue = false, Order = 2)]
		[XmlElement("Contact")]
		public ContactItemType Contact
		{
			get
			{
				return this.contact;
			}
			set
			{
				this.contact = value;
			}
		}

		private EmailAddressWrapper mailbox;

		private ContactItemType contact;
	}
}
