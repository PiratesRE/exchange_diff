using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActiveAnchorContext : AnchorContext
	{
		public ActiveAnchorContext(string applicationName, OrganizationCapability anchorCapability, OrganizationCapability activeCapability) : base(applicationName, anchorCapability)
		{
			this.activeCapability = activeCapability;
		}

		public override OrganizationCapability ActiveCapability
		{
			get
			{
				return this.activeCapability;
			}
		}

		public override CacheEntryBase CreateCacheEntry(ADUser user)
		{
			return new ToggleableCapabilityCacheEntry(this, user);
		}

		private OrganizationCapability activeCapability;
	}
}
