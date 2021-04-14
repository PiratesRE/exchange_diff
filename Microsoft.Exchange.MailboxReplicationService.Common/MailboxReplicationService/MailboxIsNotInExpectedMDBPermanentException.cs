using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxIsNotInExpectedMDBPermanentException : MailboxReplicationPermanentException
	{
		public MailboxIsNotInExpectedMDBPermanentException(Guid mdbGuid) : base(MrsStrings.SourceMailboxIsNotInSourceMDB(mdbGuid))
		{
			this.mdbGuid = mdbGuid;
		}

		public MailboxIsNotInExpectedMDBPermanentException(Guid mdbGuid, Exception innerException) : base(MrsStrings.SourceMailboxIsNotInSourceMDB(mdbGuid), innerException)
		{
			this.mdbGuid = mdbGuid;
		}

		protected MailboxIsNotInExpectedMDBPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
