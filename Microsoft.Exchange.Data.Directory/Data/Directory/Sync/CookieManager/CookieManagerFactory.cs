using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class CookieManagerFactory
	{
		public static CookieManagerFactory Default
		{
			get
			{
				CookieManagerFactory result;
				if ((result = CookieManagerFactory.defaultInstance) == null)
				{
					result = (CookieManagerFactory.defaultInstance = new CookieManagerFactory.DefaultCookieManagerFactory());
				}
				return result;
			}
			set
			{
				CookieManagerFactory.defaultInstance = value;
			}
		}

		public abstract CookieManager GetCookieManager(ForwardSyncCookieType cookieType, string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval);

		private static CookieManagerFactory defaultInstance;

		internal sealed class DefaultCookieManagerFactory : CookieManagerFactory
		{
			public override CookieManager GetCookieManager(ForwardSyncCookieType cookieType, string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval)
			{
				SyncServiceInstance syncServiceInstance = ServiceInstanceId.GetSyncServiceInstance(serviceInstanceName);
				switch (cookieType)
				{
				case ForwardSyncCookieType.RecipientIncremental:
					if (syncServiceInstance != null && syncServiceInstance.IsMultiObjectCookieEnabled)
					{
						return new MsoMultiObjectCookieManager(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval, cookieType);
					}
					return new MsoRecipientMainStreamCookieManager(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval);
				case ForwardSyncCookieType.CompanyIncremental:
					if (syncServiceInstance != null && syncServiceInstance.IsMultiObjectCookieEnabled)
					{
						return new MsoMultiObjectCookieManager(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval, cookieType);
					}
					return new MsoCompanyMainStreamCookieManager(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval);
				default:
					throw new InvalidOperationException("Cookie type not supported");
				}
			}
		}
	}
}
