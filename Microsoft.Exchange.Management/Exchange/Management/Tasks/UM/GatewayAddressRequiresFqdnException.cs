using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GatewayAddressRequiresFqdnException : LocalizedException
	{
		public GatewayAddressRequiresFqdnException() : base(Strings.GatewayAddressRequiresFqdn)
		{
		}

		public GatewayAddressRequiresFqdnException(Exception innerException) : base(Strings.GatewayAddressRequiresFqdn, innerException)
		{
		}

		protected GatewayAddressRequiresFqdnException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
