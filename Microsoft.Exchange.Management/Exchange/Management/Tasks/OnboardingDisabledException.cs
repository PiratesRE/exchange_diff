using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OnboardingDisabledException : MailboxReplicationPermanentException
	{
		public OnboardingDisabledException() : base(Strings.ErrorOnboardingDisabled)
		{
		}

		public OnboardingDisabledException(Exception innerException) : base(Strings.ErrorOnboardingDisabled, innerException)
		{
		}

		protected OnboardingDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
