using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.InfoWorker.Common.OOF;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AutoReplyConfiguration : BaseRow
	{
		public AutoReplyConfiguration(MailboxAutoReplyConfiguration mailboxAutoReplyConfiguration) : base(mailboxAutoReplyConfiguration)
		{
			this.MailboxAutoReplyConfiguration = mailboxAutoReplyConfiguration;
		}

		public MailboxAutoReplyConfiguration MailboxAutoReplyConfiguration { get; private set; }

		[DataMember]
		public string AutoReplyStateDisabled
		{
			get
			{
				return (this.MailboxAutoReplyConfiguration.AutoReplyState == OofState.Disabled).ToJsonString(null);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AutoReplyStateScheduled
		{
			get
			{
				return this.MailboxAutoReplyConfiguration.AutoReplyState == OofState.Scheduled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StartTime
		{
			get
			{
				return this.MailboxAutoReplyConfiguration.StartTime.LocalToUserDateTimeGeneralFormatString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string EndTime
		{
			get
			{
				return this.MailboxAutoReplyConfiguration.EndTime.LocalToUserDateTimeGeneralFormatString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string InternalMessage
		{
			get
			{
				return TextConverterHelper.SanitizeHtml(this.MailboxAutoReplyConfiguration.InternalMessage);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ExternalAudience
		{
			get
			{
				return this.MailboxAutoReplyConfiguration.ExternalAudience != Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.None;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ExternalAudienceKnownOnly
		{
			get
			{
				return (this.MailboxAutoReplyConfiguration.ExternalAudience == Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.Known).ToJsonString(null);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ExternalMessage
		{
			get
			{
				return TextConverterHelper.SanitizeHtml(this.MailboxAutoReplyConfiguration.ExternalMessage);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
