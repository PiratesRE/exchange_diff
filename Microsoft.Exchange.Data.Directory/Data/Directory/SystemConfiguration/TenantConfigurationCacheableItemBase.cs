using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class TenantConfigurationCacheableItemBase : CachableItem
	{
		public abstract bool InitializeWithoutRegistration(IConfigurationSession adSession, bool allowExceptions);

		public abstract bool TryInitialize(OrganizationId organizationId, CacheNotificationHandler cacheNotificationHandler, object state);

		public abstract bool Initialize(OrganizationId organizationId, CacheNotificationHandler cacheNotificationHandler, object state);

		public virtual void UnregisterChangeNotification()
		{
		}
	}
}
