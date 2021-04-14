using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MdbIsOfflineTransientException : MailboxReplicationTransientException
	{
		public MdbIsOfflineTransientException(Guid mdbGuid) : base(MrsStrings.MdbIsOffline(mdbGuid))
		{
			this.mdbGuid = mdbGuid;
		}

		public MdbIsOfflineTransientException(Guid mdbGuid, Exception innerException) : base(MrsStrings.MdbIsOffline(mdbGuid), innerException)
		{
			this.mdbGuid = mdbGuid;
		}

		protected MdbIsOfflineTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
