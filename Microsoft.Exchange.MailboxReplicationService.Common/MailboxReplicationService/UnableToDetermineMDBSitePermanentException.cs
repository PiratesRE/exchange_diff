using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToDetermineMDBSitePermanentException : MailboxReplicationPermanentException
	{
		public UnableToDetermineMDBSitePermanentException(Guid mdbGuid) : base(MrsStrings.UnableToDetermineMDBSite(mdbGuid))
		{
			this.mdbGuid = mdbGuid;
		}

		public UnableToDetermineMDBSitePermanentException(Guid mdbGuid, Exception innerException) : base(MrsStrings.UnableToDetermineMDBSite(mdbGuid), innerException)
		{
			this.mdbGuid = mdbGuid;
		}

		protected UnableToDetermineMDBSitePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbGuid = (Guid)info.GetValue("mdbGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbGuid", this.mdbGuid);
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		private readonly Guid mdbGuid;
	}
}
