using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal class CachedRegistryReader : IRegistryReader
	{
		private CachedRegistryReader()
		{
			if (CachedRegistryReader.baseKeyMap == null)
			{
				CachedRegistryReader.baseKeyMap = new Dictionary<string, RegistryKey>();
				CachedRegistryReader.baseKeyMap.Add(Registry.ClassesRoot.Name, Registry.ClassesRoot);
				CachedRegistryReader.baseKeyMap.Add(Registry.CurrentConfig.Name, Registry.CurrentConfig);
				CachedRegistryReader.baseKeyMap.Add(Registry.CurrentUser.Name, Registry.CurrentUser);
				CachedRegistryReader.baseKeyMap.Add(Registry.LocalMachine.Name, Registry.LocalMachine);
				CachedRegistryReader.baseKeyMap.Add(Registry.PerformanceData.Name, Registry.PerformanceData);
				CachedRegistryReader.baseKeyMap.Add(Registry.Users.Name, Registry.Users);
			}
			int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", "RegCacheExpirationInSeconds", 300);
			this.cachedList = new CachedList<object, string>(delegate(string str)
			{
				string[] array = CachedRegistryReader.SeperateKey(str);
				return RegistryReader.Instance.GetValue<object>(CachedRegistryReader.baseKeyMap[array[0]], array[1], array[2], null);
			}, TimeSpan.FromSeconds((double)value));
		}

		public static IRegistryReader Instance
		{
			get
			{
				if (CachedRegistryReader.cachedRegReaderInstance == null)
				{
					lock (CachedRegistryReader.instanceCreationLock)
					{
						CachedRegistryReader.cachedRegReaderInstance = new CachedRegistryReader();
					}
				}
				return CachedRegistryReader.cachedRegReaderInstance;
			}
		}

		public string[] GetSubKeyNames(RegistryKey baseKey, string subkeyName)
		{
			return RegistryReader.Instance.GetSubKeyNames(baseKey, subkeyName);
		}

		public T GetValue<T>(RegistryKey baseKey, string subkeyName, string valueName, T defaultValue)
		{
			object value = this.cachedList.GetValue(CachedRegistryReader.MakeKey(baseKey, subkeyName, valueName));
			if (value != null)
			{
				return (T)((object)value);
			}
			return defaultValue;
		}

		private static string MakeKey(RegistryKey baseKey, string subkeyName, string valueName)
		{
			return string.Join("`", new string[]
			{
				baseKey.Name,
				subkeyName,
				valueName
			});
		}

		private static string[] SeperateKey(string key)
		{
			return key.Split(new char[]
			{
				'`'
			});
		}

		private static IRegistryReader cachedRegReaderInstance = null;

		private static object instanceCreationLock = new object();

		private static Dictionary<string, RegistryKey> baseKeyMap = null;

		private CachedList<object, string> cachedList;
	}
}
