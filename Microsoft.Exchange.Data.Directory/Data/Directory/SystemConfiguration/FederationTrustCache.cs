using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FederationTrustCache
	{
		public static event FederationTrustCache.FederationTrustChangeHandler Change;

		public static IEnumerable<FederationTrust> GetFederationTrusts()
		{
			FederationTrustCache.InitializeIfNeeded();
			return new List<FederationTrust>(FederationTrustCache.dictionaryByADObjectId.Values);
		}

		public static FederationTrust GetFederationTrust(ADObjectId id)
		{
			FederationTrustCache.InitializeIfNeeded();
			FederationTrust result;
			if (FederationTrustCache.dictionaryByADObjectId.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public static FederationTrust GetFederationTrust(string name)
		{
			FederationTrustCache.InitializeIfNeeded();
			FederationTrust result;
			if (FederationTrustCache.dictionaryByCommonName.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		internal static void Initialize()
		{
			FederationTrustCache.LoadFederationTrust();
		}

		private static void InitializeIfNeeded()
		{
			if (!FederationTrustCache.IsInitialized())
			{
				lock (FederationTrustCache.locker)
				{
					if (!FederationTrustCache.IsInitialized())
					{
						FederationTrustCache.timeout = DateTime.UtcNow + FederationTrustCache.ExpirationTime;
						try
						{
							FederationTrustCache.LoadFederationTrust();
							FederationTrustCache.SubscribeForNotifications();
						}
						catch (LocalizedException arg)
						{
							FederationTrustCache.Tracer.TraceError<LocalizedException>(0L, "FederationTrustCache: Unable to initialize due exception: {0}", arg);
						}
						if (FederationTrustCache.notification != null)
						{
							FederationTrustCache.timeout = DateTime.MaxValue;
						}
						FederationTrustCache.initialized = true;
					}
				}
			}
		}

		private static bool IsInitialized()
		{
			return !(DateTime.UtcNow > FederationTrustCache.timeout) && FederationTrustCache.initialized;
		}

		private static void LoadFederationTrust()
		{
			FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: Searching for federation trust configuration in AD");
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 206, "LoadFederationTrust", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\FederationTrustCache.cs");
			FederationTrust[] array = tenantOrTopologyConfigurationSession.Find<FederationTrust>(null, QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length == 0)
			{
				FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: no federation trust has been setup");
				return;
			}
			FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: loading {0} FederationTrust objects.");
			Dictionary<ADObjectId, FederationTrust> dictionary = new Dictionary<ADObjectId, FederationTrust>(1);
			Dictionary<string, FederationTrust> dictionary2 = new Dictionary<string, FederationTrust>(1);
			foreach (FederationTrust federationTrust in array)
			{
				dictionary.Add(federationTrust.Id, federationTrust);
				dictionary2.Add(federationTrust.Name, federationTrust);
			}
			FederationTrustCache.dictionaryByADObjectId = dictionary;
			FederationTrustCache.dictionaryByCommonName = dictionary2;
		}

		private static void SubscribeForNotifications()
		{
			if (FederationTrustCache.notification != null)
			{
				return;
			}
			FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: getting federation trust container in AD.");
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 260, "SubscribeForNotifications", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\FederationTrustCache.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Federation Trusts");
			FederationTrustCache.Tracer.TraceDebug<ADObjectId>(0L, "FederationTrustCache: found federation trust configuration object in AD. {0}", childId);
			FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: registering for configuration changes in AD");
			FederationTrustCache.notification = ADNotificationAdapter.RegisterChangeNotification<FederationTrust>(childId, new ADNotificationCallback(FederationTrustCache.NotificationHandler));
			FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: successfully registered for federation trust configuration changes in AD");
		}

		private static void NotificationHandler(ADNotificationEventArgs args)
		{
			FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: changes detected in configuration in AD.");
			try
			{
				FederationTrustCache.LoadFederationTrust();
			}
			catch (LocalizedException arg)
			{
				FederationTrustCache.Tracer.TraceError<LocalizedException>(0L, "FederationTrustCache: failed to read federation trust from AD due exception: {0}", arg);
				return;
			}
			if (FederationTrustCache.Change != null)
			{
				FederationTrustCache.Tracer.TraceDebug(0L, "FederationTrustCache: notifying subscribers of change.");
				FederationTrustCache.Change();
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(5.0);

		private static DateTime timeout;

		private static bool initialized;

		private static object locker = new object();

		private static ADNotificationRequestCookie notification;

		private static Dictionary<ADObjectId, FederationTrust> dictionaryByADObjectId = new Dictionary<ADObjectId, FederationTrust>();

		private static Dictionary<string, FederationTrust> dictionaryByCommonName = new Dictionary<string, FederationTrust>();

		public delegate void FederationTrustChangeHandler();
	}
}
