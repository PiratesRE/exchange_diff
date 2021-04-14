using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetRequestKeyComparer : IEqualityComparer<SafetyNetRequestKey>
	{
		public static SafetyNetRequestKeyComparer Instance
		{
			get
			{
				return SafetyNetRequestKeyComparer.s_instance;
			}
		}

		private SafetyNetRequestKeyComparer()
		{
		}

		public bool Equals(SafetyNetRequestKey x, SafetyNetRequestKey y)
		{
			return SharedHelper.StringIEquals(x.ServerName, y.ServerName) && SharedHelper.StringIEquals(x.UniqueStr, y.UniqueStr) && object.Equals(x.RequestCreationTimeUtc, y.RequestCreationTimeUtc);
		}

		public int GetHashCode(SafetyNetRequestKey key)
		{
			return key.ServerName.GetHashCode() ^ key.RequestCreationTimeUtc.GetHashCode() ^ key.UniqueStr.GetHashCode();
		}

		private static readonly SafetyNetRequestKeyComparer s_instance = new SafetyNetRequestKeyComparer();
	}
}
