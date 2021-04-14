using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesException : LocalizedException
	{
		public ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesException(string ip) : base(Strings.ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesId(ip))
		{
			this.ip = ip;
		}

		public ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesException(string ip, Exception innerException) : base(Strings.ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesId(ip), innerException)
		{
			this.ip = ip;
		}

		protected ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesException(SerializationInfo info, StreamingContext context) : base(info, context)
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
