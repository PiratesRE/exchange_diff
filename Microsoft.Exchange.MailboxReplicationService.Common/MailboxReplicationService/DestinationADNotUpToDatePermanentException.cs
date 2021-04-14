using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DestinationADNotUpToDatePermanentException : MailboxReplicationPermanentException
	{
		public DestinationADNotUpToDatePermanentException(Guid mbxGuid) : base(MrsStrings.DestinationADNotUpToDate(mbxGuid))
		{
			this.mbxGuid = mbxGuid;
		}

		public DestinationADNotUpToDatePermanentException(Guid mbxGuid, Exception innerException) : base(MrsStrings.DestinationADNotUpToDate(mbxGuid), innerException)
		{
			this.mbxGuid = mbxGuid;
		}

		protected DestinationADNotUpToDatePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
