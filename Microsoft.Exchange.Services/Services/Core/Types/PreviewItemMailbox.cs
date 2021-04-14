using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "PreviewItemMailbox", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PreviewItemMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PreviewItemMailbox
	{
		public PreviewItemMailbox()
		{
		}

		public PreviewItemMailbox(string mailboxId, string primarySmtpAddress)
		{
			this.MailboxId = mailboxId;
			this.PrimarySmtpAddress = primarySmtpAddress;
		}

		[DataMember(Name = "MailboxId", IsRequired = true)]
		[XmlElement("MailboxId")]
		public string MailboxId { get; set; }

		[DataMember(Name = "PrimarySmtpAddress", IsRequired = true)]
		[XmlElement("PrimarySmtpAddress")]
		public string PrimarySmtpAddress
		{
			get
			{
				if (string.IsNullOrEmpty(this.primarySmtpAddres))
				{
					return string.Empty;
				}
				return this.primarySmtpAddres;
			}
			set
			{
				this.primarySmtpAddres = value;
			}
		}

		private string primarySmtpAddres;
	}
}
