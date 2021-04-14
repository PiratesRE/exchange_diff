using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxNotSyncedPermanentException : MailboxReplicationPermanentException
	{
		public MailboxNotSyncedPermanentException(Guid mbxGuid) : base(MrsStrings.MailboxNotSynced(mbxGuid))
		{
			this.mbxGuid = mbxGuid;
		}

		public MailboxNotSyncedPermanentException(Guid mbxGuid, Exception innerException) : base(MrsStrings.MailboxNotSynced(mbxGuid), innerException)
		{
			this.mbxGuid = mbxGuid;
		}

		protected MailboxNotSyncedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxGuid = (Guid)info.GetValue("mbxGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxGuid", this.mbxGuid);
		}

		public Guid MbxGuid
		{
			get
			{
				return this.mbxGuid;
			}
		}

		private readonly Guid mbxGuid;
	}
}
