using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FailedSearchMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "FailedSearchMailbox", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FailedSearchMailbox
	{
		public FailedSearchMailbox()
		{
		}

		internal FailedSearchMailbox(string mailbox, int errorCode, string errorMessage)
		{
			this.mailbox = mailbox;
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
		}

		[DataMember(Name = "Mailbox", IsRequired = true)]
		[XmlElement("Mailbox")]
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

		[DataMember(Name = "ErrorCode", IsRequired = true)]
		[XmlElement("ErrorCode")]
		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		[DataMember(Name = "ErrorMessage", IsRequired = true)]
		[XmlElement("ErrorMessage")]
		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.errorMessage = value;
			}
		}

		[XmlElement("IsArchive")]
		[DataMember(Name = "IsArchive", IsRequired = true)]
		public bool IsArchive
		{
			get
			{
				return this.isArchive;
			}
			set
			{
				this.isArchive = value;
			}
		}

		private string mailbox;

		private int errorCode;

		private string errorMessage;

		private bool isArchive;
	}
}
