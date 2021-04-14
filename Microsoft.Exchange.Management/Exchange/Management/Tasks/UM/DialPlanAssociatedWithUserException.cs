using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DialPlanAssociatedWithUserException : LocalizedException
	{
		public DialPlanAssociatedWithUserException() : base(Strings.DialPlanAssociatedWithUserException)
		{
		}

		public DialPlanAssociatedWithUserException(Exception innerException) : base(Strings.DialPlanAssociatedWithUserException, innerException)
		{
		}

		protected DialPlanAssociatedWithUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
