using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GatewayFqdnNotInAcceptedDomainException : LocalizedException
	{
		public GatewayFqdnNotInAcceptedDomainException() : base(Strings.GatewayFqdnNotInAcceptedDomain)
		{
		}

		public GatewayFqdnNotInAcceptedDomainException(Exception innerException) : base(Strings.GatewayFqdnNotInAcceptedDomain, innerException)
		{
		}

		protected GatewayFqdnNotInAcceptedDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
