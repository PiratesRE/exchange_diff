using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ReadingPaneConfiguration : MessagingConfigurationBase
	{
		public ReadingPaneConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
		}

		[DataMember]
		public string PreviewMarkAsReadBehavior
		{
			get
			{
				return base.MailboxMessageConfiguration.PreviewMarkAsReadBehavior.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int PreviewMarkAsReadDelaytime
		{
			get
			{
				return base.MailboxMessageConfiguration.PreviewMarkAsReadDelaytime;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string EmailComposeMode
		{
			get
			{
				return base.MailboxMessageConfiguration.EmailComposeMode.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
