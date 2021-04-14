using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateItemInGatewayIpAddressListException : LocalizedException
	{
		public DuplicateItemInGatewayIpAddressListException(string ip) : base(Strings.DuplicateItemInGatewayIpAddressListId(ip))
		{
			this.ip = ip;
		}

		public DuplicateItemInGatewayIpAddressListException(string ip, Exception innerException) : base(Strings.DuplicateItemInGatewayIpAddressListId(ip), innerException)
		{
			this.ip = ip;
		}

		protected DuplicateItemInGatewayIpAddressListException(SerializationInfo info, StreamingContext context) : base(info, context)
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
