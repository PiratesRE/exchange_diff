using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationPartitionMailboxInvalidException : LocalizedException
	{
		public MigrationPartitionMailboxInvalidException(string mailbox) : base(Strings.MigrationPartitionMailboxInvalid(mailbox))
		{
			this.mailbox = mailbox;
		}

		public MigrationPartitionMailboxInvalidException(string mailbox, Exception innerException) : base(Strings.MigrationPartitionMailboxInvalid(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected MigrationPartitionMailboxInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private readonly string mailbox;
	}
}
