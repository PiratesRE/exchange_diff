using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotCreateGatewayForHostedSipDialPlanException : LocalizedException
	{
		public CannotCreateGatewayForHostedSipDialPlanException() : base(Strings.CannotCreateGatewayForHostedSipDialPlan)
		{
		}

		public CannotCreateGatewayForHostedSipDialPlanException(Exception innerException) : base(Strings.CannotCreateGatewayForHostedSipDialPlan, innerException)
		{
		}

		protected CannotCreateGatewayForHostedSipDialPlanException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
