using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NotSupportedWithServerVersionException : StoragePermanentException
	{
		public NotSupportedWithServerVersionException(string mailboxId, int mailboxVersion, int serverVersion) : base(ServerStrings.idNotSupportedWithServerVersionException(mailboxId, mailboxVersion, serverVersion))
		{
			this.mailboxId = mailboxId;
			this.mailboxVersion = mailboxVersion;
			this.serverVersion = serverVersion;
		}

		public NotSupportedWithServerVersionException(string mailboxId, int mailboxVersion, int serverVersion, Exception innerException) : base(ServerStrings.idNotSupportedWithServerVersionException(mailboxId, mailboxVersion, serverVersion), innerException)
		{
			this.mailboxId = mailboxId;
			this.mailboxVersion = mailboxVersion;
			this.serverVersion = serverVersion;
		}

		protected NotSupportedWithServerVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxId = (string)info.GetValue("mailboxId", typeof(string));
			this.mailboxVersion = (int)info.GetValue("mailboxVersion", typeof(int));
			this.serverVersion = (int)info.GetValue("serverVersion", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxId", this.mailboxId);
			info.AddValue("mailboxVersion", this.mailboxVersion);
			info.AddValue("serverVersion", this.serverVersion);
		}

		public string MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		public int MailboxVersion
		{
			get
			{
				return this.mailboxVersion;
			}
		}

		public int ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		private readonly string mailboxId;

		private readonly int mailboxVersion;

		private readonly int serverVersion;
	}
}
