using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IPGatewayAlreadyExistsException : LocalizedException
	{
		public IPGatewayAlreadyExistsException(string ipaddress) : base(Strings.ExceptionIPGatewayAlreadyExists(ipaddress))
		{
			this.ipaddress = ipaddress;
		}

		public IPGatewayAlreadyExistsException(string ipaddress, Exception innerException) : base(Strings.ExceptionIPGatewayAlreadyExists(ipaddress), innerException)
		{
			this.ipaddress = ipaddress;
		}

		protected IPGatewayAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
