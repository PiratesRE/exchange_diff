using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class IPRangeRemote : IPRange
	{
		public IPRangeRemote(IPRange ipRange) : base(ipRange.LowerBound, ipRange.UpperBound)
		{
			this.size = base.Size;
		}

		public static int Compare(IPRangeRemote v1, IPRangeRemote v2)
		{
			return IPvxAddress.Compare(v1.size, v2.size);
		}

		private readonly IPvxAddress size;
	}
}
