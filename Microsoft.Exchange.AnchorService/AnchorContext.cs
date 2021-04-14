using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorContext : SimpleContext
	{
		public AnchorContext(string applicationName, OrganizationCapability anchorCapability) : base(applicationName)
		{
			this.AnchorCapability = anchorCapability;
		}

		internal AnchorContext(string applicationName, OrganizationCapability anchorCapability, AnchorConfig config) : base(applicationName, config)
		{
			this.AnchorCapability = anchorCapability;
		}

		protected AnchorContext(OrganizationCapability anchorCapability)
		{
			this.AnchorCapability = anchorCapability;
		}

		public OrganizationCapability AnchorCapability { get; protected set; }

		public virtual OrganizationCapability ActiveCapability
		{
			get
			{
				return this.AnchorCapability;
			}
		}

		public virtual CacheEntryBase CreateCacheEntry(ADUser user)
		{
			return new CapabilityCacheEntry(this, user);
		}

		public virtual CacheProcessorBase[] CreateCacheComponents(WaitHandle stopEvent)
		{
			return new CacheProcessorBase[]
			{
				new CacheScanner(this, stopEvent),
				new CacheScheduler(this, stopEvent)
			};
		}

		public virtual TimedOperationRunner CreateOperationRunner()
		{
			return new TimedOperationRunner(base.Logger, base.Config.GetConfig<TimeSpan>("SlowOperationThreshold"));
		}
	}
}
