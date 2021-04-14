using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class RopIdComparer : IComparer<RopId>, IEqualityComparer<RopId>
	{
		public int Compare(RopId x, RopId y)
		{
			return (int)(y - x);
		}

		public bool Equals(RopId x, RopId y)
		{
			return x == y;
		}

		public int GetHashCode(RopId tag)
		{
			return (int)tag;
		}
	}
}
