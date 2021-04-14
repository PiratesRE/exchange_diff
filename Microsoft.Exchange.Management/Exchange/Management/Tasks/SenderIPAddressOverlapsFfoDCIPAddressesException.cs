using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SenderIPAddressOverlapsFfoDCIPAddressesException : LocalizedException
	{
		public SenderIPAddressOverlapsFfoDCIPAddressesException(string ipRange) : base(Strings.SenderIPAddressOverlapsFfoDCIPAddressesId(ipRange))
		{
			this.ipRange = ipRange;
		}

		public SenderIPAddressOverlapsFfoDCIPAddressesException(string ipRange, Exception innerException) : base(Strings.SenderIPAddressOverlapsFfoDCIPAddressesId(ipRange), innerException)
		{
			this.ipRange = ipRange;
		}

		protected SenderIPAddressOverlapsFfoDCIPAddressesException(SerializationInfo info, StreamingContext context) : base(info, context)
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
