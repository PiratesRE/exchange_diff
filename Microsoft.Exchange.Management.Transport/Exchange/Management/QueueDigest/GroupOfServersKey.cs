using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal abstract class GroupOfServersKey
	{
		public static GroupOfServersKey CreateFromDag(ADObjectId dagId)
		{
			return new DagGroupOfServersKey(dagId);
		}

		public static GroupOfServersKey CreateFromSite(ADObjectId siteId, int majorVersion)
		{
			return new SiteGroupOfServersKey(siteId, majorVersion);
		}
	}
}
