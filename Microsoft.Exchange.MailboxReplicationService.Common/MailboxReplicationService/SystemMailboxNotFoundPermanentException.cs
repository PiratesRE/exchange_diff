using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SystemMailboxNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public SystemMailboxNotFoundPermanentException(string systemMailboxName) : base(MrsStrings.SystemMailboxNotFound(systemMailboxName))
		{
			this.systemMailboxName = systemMailboxName;
		}

		public SystemMailboxNotFoundPermanentException(string systemMailboxName, Exception innerException) : base(MrsStrings.SystemMailboxNotFound(systemMailboxName), innerException)
		{
			this.systemMailboxName = systemMailboxName;
		}

		protected SystemMailboxNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.systemMailboxName = (string)info.GetValue("systemMailboxName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("systemMailboxName", this.systemMailboxName);
		}

		public string SystemMailboxName
		{
			get
			{
				return this.systemMailboxName;
			}
		}

		private readonly string systemMailboxName;
	}
}
