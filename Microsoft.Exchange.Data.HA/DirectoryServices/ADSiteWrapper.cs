using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADSiteWrapper : ADObjectWrapperBase, IADSite, IADObjectCommon
	{
		private ADSiteWrapper(ADSite adSite) : base(adSite)
		{
		}

		public static ADSiteWrapper CreateWrapper(ADSite adSite)
		{
			if (adSite == null)
			{
				return null;
			}
			return new ADSiteWrapper(adSite);
		}
	}
}
