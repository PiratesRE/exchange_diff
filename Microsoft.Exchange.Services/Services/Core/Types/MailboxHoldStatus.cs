using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "MailboxHoldStatus", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "MailboxHoldStatusType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailboxHoldStatus
	{
		public MailboxHoldStatus()
		{
		}

		internal MailboxHoldStatus(string mailbox, HoldStatus status, string additionalInfo)
		{
			this.mailbox = mailbox;
			this.status = status;
			this.additionalInfo = additionalInfo;
		}

		[XmlElement("Mailbox")]
		[DataMember(Name = "Mailbox", IsRequired = true)]
		public string Mailbox
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

		[XmlElement("Status")]
		[IgnoreDataMember]
		public HoldStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		[DataMember(Name = "Status", IsRequired = true)]
		[XmlIgnore]
		public string StatusString
		{
			get
			{
				return EnumUtilities.ToString<HoldStatus>(this.status);
			}
			set
			{
				this.status = EnumUtilities.Parse<HoldStatus>(value);
			}
		}

		[XmlElement("AdditionalInfo")]
		[DataMember(Name = "AdditionalInfo", IsRequired = false)]
		public string AdditionalInfo
		{
			get
			{
				return this.additionalInfo;
			}
			set
			{
				this.additionalInfo = value;
			}
		}

		private string mailbox;

		private HoldStatus status;

		private string additionalInfo;
	}
}
