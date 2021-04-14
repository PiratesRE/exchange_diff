using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EMailSignatureConfiguration : MessagingConfigurationBase
	{
		public EMailSignatureConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
		}

		[DataMember]
		public string SignatureHtml
		{
			get
			{
				return TextConverterHelper.SanitizeHtml(base.MailboxMessageConfiguration.SignatureHtml);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AutoAddSignature
		{
			get
			{
				return base.MailboxMessageConfiguration.AutoAddSignature;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
