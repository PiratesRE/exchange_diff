using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxVersionTooLowException : DataSourceOperationException
	{
		public MailboxVersionTooLowException(string mailbox, string expectedVersion, string actualVersion) : base(ServerStrings.MailboxVersionTooLow(mailbox, expectedVersion, actualVersion))
		{
			this.mailbox = mailbox;
			this.expectedVersion = expectedVersion;
			this.actualVersion = actualVersion;
		}

		public MailboxVersionTooLowException(string mailbox, string expectedVersion, string actualVersion, Exception innerException) : base(ServerStrings.MailboxVersionTooLow(mailbox, expectedVersion, actualVersion), innerException)
		{
			this.mailbox = mailbox;
			this.expectedVersion = expectedVersion;
			this.actualVersion = actualVersion;
		}

		protected MailboxVersionTooLowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
			this.expectedVersion = (string)info.GetValue("expectedVersion", typeof(string));
			this.actualVersion = (string)info.GetValue("actualVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
			info.AddValue("expectedVersion", this.expectedVersion);
			info.AddValue("actualVersion", this.actualVersion);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public string ExpectedVersion
		{
			get
			{
				return this.expectedVersion;
			}
		}

		public string ActualVersion
		{
			get
			{
				return this.actualVersion;
			}
		}

		private readonly string mailbox;

		private readonly string expectedVersion;

		private readonly string actualVersion;
	}
}
