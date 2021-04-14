using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveDefaultSiteMailboxProvisioningPolicyException : LocalizedException
	{
		public CannotRemoveDefaultSiteMailboxProvisioningPolicyException() : base(Strings.CannotRemoveDefaultSiteMailboxProvisioningPolicyException)
		{
		}

		public CannotRemoveDefaultSiteMailboxProvisioningPolicyException(Exception innerException) : base(Strings.CannotRemoveDefaultSiteMailboxProvisioningPolicyException, innerException)
		{
		}

		protected CannotRemoveDefaultSiteMailboxProvisioningPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
