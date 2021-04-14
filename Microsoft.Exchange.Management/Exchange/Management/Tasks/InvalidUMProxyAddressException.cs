using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidUMProxyAddressException : LocalizedException
	{
		public InvalidUMProxyAddressException(string proxyAddress) : base(Strings.InvalidUMProxyAddressException(proxyAddress))
		{
			this.proxyAddress = proxyAddress;
		}

		public InvalidUMProxyAddressException(string proxyAddress, Exception innerException) : base(Strings.InvalidUMProxyAddressException(proxyAddress), innerException)
		{
			this.proxyAddress = proxyAddress;
		}

		protected InvalidUMProxyAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.proxyAddress = (string)info.GetValue("proxyAddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("proxyAddress", this.proxyAddress);
		}

		public string ProxyAddress
		{
			get
			{
				return this.proxyAddress;
			}
		}

		private readonly string proxyAddress;
	}
}
