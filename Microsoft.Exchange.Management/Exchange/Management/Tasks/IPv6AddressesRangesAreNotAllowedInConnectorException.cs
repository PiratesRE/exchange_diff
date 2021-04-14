using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IPv6AddressesRangesAreNotAllowedInConnectorException : LocalizedException
	{
		public IPv6AddressesRangesAreNotAllowedInConnectorException(string ipRange) : base(Strings.IPv6AddressesRangesAreNotAllowedInConnectorId(ipRange))
		{
			this.ipRange = ipRange;
		}

		public IPv6AddressesRangesAreNotAllowedInConnectorException(string ipRange, Exception innerException) : base(Strings.IPv6AddressesRangesAreNotAllowedInConnectorId(ipRange), innerException)
		{
			this.ipRange = ipRange;
		}

		protected IPv6AddressesRangesAreNotAllowedInConnectorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ipRange = (string)info.GetValue("ipRange", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ipRange", this.ipRange);
		}

		public string IpRange
		{
			get
			{
				return this.ipRange;
			}
		}

		private readonly string ipRange;
	}
}
