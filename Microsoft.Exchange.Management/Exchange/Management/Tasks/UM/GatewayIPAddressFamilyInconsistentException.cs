using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GatewayIPAddressFamilyInconsistentException : LocalizedException
	{
		public GatewayIPAddressFamilyInconsistentException() : base(Strings.GatewayIPAddressFamilyInconsistentException)
		{
		}

		public GatewayIPAddressFamilyInconsistentException(Exception innerException) : base(Strings.GatewayIPAddressFamilyInconsistentException, innerException)
		{
		}

		protected GatewayIPAddressFamilyInconsistentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
