using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal static class DeviceBehaviorCache
	{
		internal static void Start()
		{
			if (GlobalSettings.DeviceBehaviorCacheTimeout > 0)
			{
				lock (DeviceBehaviorCache.synchronizationObject)
				{
					if (DeviceBehaviorCache.deviceBehaviorCache != null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, DeviceBehaviorCache.deviceBehaviorCache, "DeviceBehaviorCache is already started.");
					}
					else
					{
						DeviceBehaviorCache.deviceBehaviorCache = new MruDictionaryCache<string, DeviceBehavior>(GlobalSettings.DeviceBehaviorCacheInitialSize, GlobalSettings.DeviceBehaviorCacheMaxSize, GlobalSettings.DeviceBehaviorCacheTimeout);
					}
				}
			}
		}

		internal static void Stop()
		{
			lock (DeviceBehaviorCache.synchronizationObject)
			{
				if (DeviceBehaviorCache.deviceBehaviorCache != null)
				{
					DeviceBehaviorCache.deviceBehaviorCache.Dispose();
					DeviceBehaviorCache.deviceBehaviorCache = null;
				}
			}
		}

		public static bool TryGetAndRemoveValue(Guid userGuid, DeviceIdentity deviceIdentity, out DeviceBehavior data)
		{
			string token = DeviceBehaviorCache.GetToken(userGuid, deviceIdentity);
			return DeviceBehaviorCache.TryGetAndRemoveValue(token, out data);
		}

		public static bool TryGetAndRemoveValue(string token, out DeviceBehavior data)
		{
			bool result;
			lock (DeviceBehaviorCache.synchronizationObject)
			{
				if (DeviceBehaviorCache.deviceBehaviorCache != null)
				{
					result = DeviceBehaviorCache.deviceBehaviorCache.TryGetAndRemoveValue(token, out data);
				}
				else
				{
					data = null;
					result = false;
				}
			}
			return result;
		}

		public static bool TryGetValue(Guid userGuid, DeviceIdentity deviceIdentity, out DeviceBehavior data)
		{
			string token = DeviceBehaviorCache.GetToken(userGuid, deviceIdentity);
			return DeviceBehaviorCache.TryGetValue(token, out data);
		}

		public static bool TryGetValue(string token, out DeviceBehavior data)
		{
			bool result;
			lock (DeviceBehaviorCache.synchronizationObject)
			{
				if (DeviceBehaviorCache.deviceBehaviorCache != null)
				{
					result = DeviceBehaviorCache.deviceBehaviorCache.TryGetValue(token, out data);
				}
				else
				{
					data = null;
					result = false;
				}
			}
			return result;
		}

		public static bool ContainsKey(Guid userGuid, DeviceIdentity deviceIdentity)
		{
			string token = DeviceBehaviorCache.GetToken(userGuid, deviceIdentity);
			return DeviceBehaviorCache.ContainsKey(token);
		}

		public static bool ContainsKey(string token)
		{
			bool result;
			lock (DeviceBehaviorCache.synchronizationObject)
			{
				if (DeviceBehaviorCache.deviceBehaviorCache != null)
				{
					result = DeviceBehaviorCache.deviceBehaviorCache.ContainsKey(token);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public static void AddOrReplace(Guid userGuid, DeviceIdentity deviceIdentity, DeviceBehavior data)
		{
			string token = DeviceBehaviorCache.GetToken(userGuid, deviceIdentity);
			DeviceBehaviorCache.AddOrReplace(token, data);
		}

		public static void AddOrReplace(string token, DeviceBehavior data)
		{
			lock (DeviceBehaviorCache.synchronizationObject)
			{
				if (DeviceBehaviorCache.deviceBehaviorCache != null)
				{
					DeviceBehaviorCache.deviceBehaviorCache[token] = data;
				}
			}
		}

		public static string GetToken(Guid userGuid, DeviceIdentity deviceIdentity)
		{
			ArgumentValidator.ThrowIfNull("userGuid", userGuid);
			ArgumentValidator.ThrowIfNull("deviceIdentity", deviceIdentity);
			return string.Concat(new object[]
			{
				userGuid.ToString(),
				'§',
				deviceIdentity.DeviceType,
				'§',
				deviceIdentity.DeviceId
			});
		}

		private static object synchronizationObject = new object();

		private static MruDictionaryCache<string, DeviceBehavior> deviceBehaviorCache;
	}
}
