using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IPv6AddressesAreNotAllowedInGatewayIPAddressesException : LocalizedException
	{
		public IPv6AddressesAreNotAllowedInGatewayIPAddressesException(string ip) : base(Strings.IPv6AddressesAreNotAllowedInGatewayIPAddressesId(ip))
		{
			this.ip = ip;
		}

		public IPv6AddressesAreNotAllowedInGatewayIPAddressesException(string ip, Exception innerException) : base(Strings.IPv6AddressesAreNotAllowedInGatewayIPAddressesId(ip), innerException)
		{
			this.ip = ip;
		}

		protected IPv6AddressesAreNotAllowedInGatewayIPAddressesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ip = (string)info.GetValue("ip", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ip", this.ip);
		}

		public string Ip
		{
			get
			{
				return this.ip;
			}
		}

		private readonly string ip;
	}
}
