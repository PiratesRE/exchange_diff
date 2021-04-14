using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIPRangeFormatException : LocalizedException
	{
		public InvalidIPRangeFormatException(string ipRange) : base(Strings.InvalidIPRangeFormatId(ipRange))
		{
			this.ipRange = ipRange;
		}

		public InvalidIPRangeFormatException(string ipRange, Exception innerException) : base(Strings.InvalidIPRangeFormatId(ipRange), innerException)
		{
			this.ipRange = ipRange;
		}

		protected InvalidIPRangeFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
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
