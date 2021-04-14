using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OffboardingDisabledException : MailboxReplicationPermanentException
	{
		public OffboardingDisabledException() : base(Strings.ErrorOffboardingDisabled)
		{
		}

		public OffboardingDisabledException(Exception innerException) : base(Strings.ErrorOffboardingDisabled, innerException)
		{
		}

		protected OffboardingDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
