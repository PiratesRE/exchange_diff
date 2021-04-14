using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DestinationMailboxNotCleanedUpPermanentException : MailboxReplicationPermanentException
	{
		public DestinationMailboxNotCleanedUpPermanentException(Guid mbxGuid) : base(MrsStrings.DestinationMailboxNotCleanedUp(mbxGuid))
		{
			this.mbxGuid = mbxGuid;
		}

		public DestinationMailboxNotCleanedUpPermanentException(Guid mbxGuid, Exception innerException) : base(MrsStrings.DestinationMailboxNotCleanedUp(mbxGuid), innerException)
		{
			this.mbxGuid = mbxGuid;
		}

		protected DestinationMailboxNotCleanedUpPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
