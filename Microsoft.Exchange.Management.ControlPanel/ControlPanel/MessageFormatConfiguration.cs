using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageFormatConfiguration : MessagingConfigurationBase
	{
		public MessageFormatConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
		}

		[DataMember]
		public bool AlwaysShowBcc
		{
			get
			{
				return base.MailboxMessageConfiguration.AlwaysShowBcc;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AlwaysShowFrom
		{
			get
			{
				return base.MailboxMessageConfiguration.AlwaysShowFrom;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DefaultFormat
		{
			get
			{
				return base.MailboxMessageConfiguration.DefaultFormat.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public FormatBarState MessageFont
		{
			get
			{
				return new FormatBarState(base.MailboxMessageConfiguration.DefaultFontName, base.MailboxMessageConfiguration.DefaultFontSize, base.MailboxMessageConfiguration.DefaultFontFlags, base.MailboxMessageConfiguration.DefaultFontColor.Remove(0, 1));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
