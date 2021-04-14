using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal class SiteGroupOfServersKey : GroupOfServersKey
	{
		public SiteGroupOfServersKey(ADObjectId siteId, int majorVersion)
		{
			this.siteId = siteId;
			this.majorVersion = majorVersion;
		}

		public override bool Equals(object other)
		{
			SiteGroupOfServersKey siteGroupOfServersKey = other as SiteGroupOfServersKey;
			return siteGroupOfServersKey != null && this.siteId.Equals(siteGroupOfServersKey.siteId) && this.majorVersion == siteGroupOfServersKey.majorVersion;
		}

		public override int GetHashCode()
		{
			return this.siteId.GetHashCode() + this.majorVersion;
		}

		public override string ToString()
		{
			return string.Format("{0}-[{1}]", this.siteId.ToString(), this.majorVersion);
		}

		private readonly ADObjectId siteId;

		private readonly int majorVersion;
	}
}
