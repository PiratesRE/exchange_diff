using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesException : LocalizedException
	{
		public InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesException(string ip) : base(Strings.InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesId(ip))
		{
			this.ip = ip;
		}

		public InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesException(string ip, Exception innerException) : base(Strings.InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesId(ip), innerException)
		{
			this.ip = ip;
		}

		protected InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesException(SerializationInfo info, StreamingContext context) : base(info, context)
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
