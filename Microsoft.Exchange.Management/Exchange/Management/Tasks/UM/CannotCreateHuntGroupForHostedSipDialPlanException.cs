using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotCreateHuntGroupForHostedSipDialPlanException : LocalizedException
	{
		public CannotCreateHuntGroupForHostedSipDialPlanException() : base(Strings.CannotCreateHuntGroupForHostedSipDialPlan)
		{
		}

		public CannotCreateHuntGroupForHostedSipDialPlanException(Exception innerException) : base(Strings.CannotCreateHuntGroupForHostedSipDialPlan, innerException)
		{
		}

		protected CannotCreateHuntGroupForHostedSipDialPlanException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
