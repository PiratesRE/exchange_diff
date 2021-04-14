using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ReadReceiptsConfiguration : MessagingConfigurationBase
	{
		public ReadReceiptsConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
		}

		[DataMember]
		public string ReadReceiptResponse
		{
			get
			{
				return base.MailboxMessageConfiguration.ReadReceiptResponse.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
