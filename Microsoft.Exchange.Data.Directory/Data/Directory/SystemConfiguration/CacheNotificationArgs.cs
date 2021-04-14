using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class CacheNotificationArgs
	{
		public CacheNotificationArgs(CacheNotificationHandler cacheNotificationHandler, OrganizationId organizationId)
		{
			this.cacheNotificationHandler = cacheNotificationHandler;
			this.organizationId = organizationId;
		}

		public CacheNotificationHandler CacheNotificationHandler
		{
			get
			{
				return this.cacheNotificationHandler;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		private CacheNotificationHandler cacheNotificationHandler;

		private OrganizationId organizationId;
	}
}
