using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IPGatewayAlreadyIPAddressExistsException : LocalizedException
	{
		public IPGatewayAlreadyIPAddressExistsException(string ipaddress) : base(Strings.ExceptionIPGatewayIPAddressAlreadyExists(ipaddress))
		{
			this.ipaddress = ipaddress;
		}

		public IPGatewayAlreadyIPAddressExistsException(string ipaddress, Exception innerException) : base(Strings.ExceptionIPGatewayIPAddressAlreadyExists(ipaddress), innerException)
		{
			this.ipaddress = ipaddress;
		}

		protected IPGatewayAlreadyIPAddressExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ipaddress = (string)info.GetValue("ipaddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ipaddress", this.ipaddress);
		}

		public string Ipaddress
		{
			get
			{
				return this.ipaddress;
			}
		}

		private readonly string ipaddress;
	}
}
