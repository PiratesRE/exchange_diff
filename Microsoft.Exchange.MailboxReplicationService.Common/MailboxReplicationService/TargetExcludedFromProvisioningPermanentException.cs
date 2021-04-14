using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TargetExcludedFromProvisioningPermanentException : MailboxReplicationPermanentException
	{
		public TargetExcludedFromProvisioningPermanentException(Guid mdbName) : base(MrsStrings.IsExcludedFromProvisioningError(mdbName))
		{
			this.mdbName = mdbName;
		}

		public TargetExcludedFromProvisioningPermanentException(Guid mdbName, Exception innerException) : base(MrsStrings.IsExcludedFromProvisioningError(mdbName), innerException)
		{
			this.mdbName = mdbName;
		}

		protected TargetExcludedFromProvisioningPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbName = (Guid)info.GetValue("mdbName", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbName", this.mdbName);
		}

		public Guid MdbName
		{
			get
			{
				return this.mdbName;
			}
		}

		private readonly Guid mdbName;
	}
}
