using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidCidrRangeException : LocalizedException
	{
		public InvalidCidrRangeException(string ipRange, int minCidrLength) : base(Strings.InvalidCidrRangeId(ipRange, minCidrLength))
		{
			this.ipRange = ipRange;
			this.minCidrLength = minCidrLength;
		}

		public InvalidCidrRangeException(string ipRange, int minCidrLength, Exception innerException) : base(Strings.InvalidCidrRangeId(ipRange, minCidrLength), innerException)
		{
			this.ipRange = ipRange;
			this.minCidrLength = minCidrLength;
		}

		protected InvalidCidrRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ipRange = (string)info.GetValue("ipRange", typeof(string));
			this.minCidrLength = (int)info.GetValue("minCidrLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ipRange", this.ipRange);
			info.AddValue("minCidrLength", this.minCidrLength);
		}

		public string IpRange
		{
			get
			{
				return this.ipRange;
			}
		}

		public int MinCidrLength
		{
			get
			{
				return this.minCidrLength;
			}
		}

		private readonly string ipRange;

		private readonly int minCidrLength;
	}
}
