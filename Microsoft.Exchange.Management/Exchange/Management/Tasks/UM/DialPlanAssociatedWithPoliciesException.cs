using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DialPlanAssociatedWithPoliciesException : LocalizedException
	{
		public DialPlanAssociatedWithPoliciesException() : base(Strings.DialPlanAssociatedWithPoliciesException)
		{
		}

		public DialPlanAssociatedWithPoliciesException(Exception innerException) : base(Strings.DialPlanAssociatedWithPoliciesException, innerException)
		{
		}

		protected DialPlanAssociatedWithPoliciesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
