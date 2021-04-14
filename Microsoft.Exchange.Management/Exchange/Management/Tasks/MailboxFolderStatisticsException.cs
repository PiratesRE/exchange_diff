using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxFolderStatisticsException : LocalizedException
	{
		public MailboxFolderStatisticsException(string mailbox, string failure) : base(Strings.MailboxFolderStatisticsException(mailbox, failure))
		{
			this.mailbox = mailbox;
			this.failure = failure;
		}

		public MailboxFolderStatisticsException(string mailbox, string failure, Exception innerException) : base(Strings.MailboxFolderStatisticsException(mailbox, failure), innerException)
		{
			this.mailbox = mailbox;
			this.failure = failure;
		}

		protected MailboxFolderStatisticsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
			info.AddValue("failure", this.failure);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string mailbox;

		private readonly string failure;
	}
}
