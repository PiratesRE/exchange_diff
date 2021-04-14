using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GlobalLocatorCache;
using Microsoft.Exchange.Security.Compliance;
using Microsoft.Exchange.Servicelets.GlobalLocatorCache.Messages;
using Microsoft.Win32;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	internal class GlobalLocatorCache
	{
		protected GlobalLocatorCache()
		{
			if (!Datacenter.IsMicrosoftHostedOnly(true))
			{
				ExTraceGlobals.ServiceTracer.TraceError((long)this.GetHashCode(), "GlobalLocatorCache was invoked unexpectedly in a non-Datacenter environment");
				throw new InvalidOperationException("Unsupported environment");
			}
			ExTraceGlobals.ServiceTracer.TraceDebug((long)this.GetHashCode(), "Creating an instance of GlobalLocatorCache");
			lock (GlobalLocatorCache.dictionaryLockObject)
			{
				if (GlobalLocatorCache.cacheLoadType == null)
				{
					GlobalLocatorCache.cacheLoadType = new CacheLoadType?(this.GetCacheLoadType());
				}
				if (GlobalLocatorCache.fileHandler == null)
				{
					string filePath = Path.Combine(GlobalLocatorCache.GetExchangeBinPath(), GlobalLocatorCache.GlobalLocatorCacheFile);
					GlobalLocatorCache.fileHandler = new FileHandler(filePath);
					GlobalLocatorCache.fileHandler.Changed += GlobalLocatorCache.ReloadOnFileChanged;
				}
				if (GlobalLocatorCache.internalTimer == null)
				{
					GlobalLocatorCache.internalTimer = new Timer(new TimerCallback(GlobalLocatorCache.ReloadOnTimer), null, 86400000, 86400000);
				}
			}
		}

		internal static ExEventLog EventLogger
		{
			get
			{
				return GlobalLocatorCache.eventLogger;
			}
		}

		internal static string GetCaseInsensitiveMd5Hash(string input)
		{
			input = input.ToLowerInvariant();
			string result;
			using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
			{
				byte[] array = messageDigestForNonCryptographicPurposes.ComputeHash(Encoding.UTF8.GetBytes(input));
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		internal bool TryFindTenantInfoInCache(string domainOrTenantId, out OfflineTenantInfo tenantInfo, out string failure)
		{
			tenantInfo = null;
			failure = null;
			string caseInsensitiveMd5Hash = GlobalLocatorCache.GetCaseInsensitiveMd5Hash(domainOrTenantId);
			if (!string.IsNullOrEmpty(caseInsensitiveMd5Hash))
			{
				lock (GlobalLocatorCache.dictionaryLockObject)
				{
					Dictionary<Guid, OfflineTenantInfo> dictionary = GlobalLocatorCache.offlineTenantMapBuckets[(int)((byte)caseInsensitiveMd5Hash[0])];
					if (dictionary == null || !dictionary.TryGetValue(new Guid(caseInsensitiveMd5Hash), out tenantInfo))
					{
						failure = "Entry not found in GlobalLocatorCache";
						return false;
					}
					return true;
				}
			}
			failure = string.Format("Cannot get valid Md5 hash value from domain name {0}", domainOrTenantId);
			return false;
		}

		protected static void LogMserveInfo(string msg, string failure, int ticksElapsed, string requestName, string partnerId)
		{
			MservProtocolLog.BeginAppend(msg, string.IsNullOrEmpty(failure) ? "Success" : "Failure", (long)ticksElapsed, failure, requestName, partnerId, "localhost", string.Empty, string.Empty);
		}

		protected static bool IsCacheReady()
		{
			if (GlobalLocatorCache.offlineTenantMapBuckets == null)
			{
				ExTraceGlobals.ServiceTracer.TraceError(0L, "The Global Locator cache is not loaded yet, please try again later.");
				return false;
			}
			return true;
		}

		protected static string GetForest(int forestId)
		{
			return GlobalLocatorCache.forestMap[forestId].ForestFqdn;
		}

		private static void ReloadOnFileChanged()
		{
			Thread.Sleep(10000);
			GlobalLocatorCache.LoadFromFile("ReloadOnFileChanged");
		}

		private static void ReloadOnTimer(object state)
		{
			GlobalLocatorCache.LoadFromFile("ReloadOnTimer");
		}

		private static void LoadFromFile(string cacheLoaderName)
		{
			int num = 0;
			int num2 = 0;
			int tickCount = Environment.TickCount;
			Dictionary<Guid, OfflineTenantInfo>[] array = new Dictionary<Guid, OfflineTenantInfo>[256];
			Dictionary<int, ForestRegionTuple> dictionary = new Dictionary<int, ForestRegionTuple>();
			string text = Path.Combine(GlobalLocatorCache.GetExchangeBinPath(), GlobalLocatorCache.GlobalLocatorCacheFile);
			string text2 = string.Empty;
			string text3 = string.Empty;
			int num3 = 2;
			int i = 1;
			long num4 = 0L;
			int num5 = 100000;
			int num6 = 0;
			int num7 = 4;
			ExTraceGlobals.ServiceTracer.TraceDebug<string>(0L, "Loading Global Locator Cache from file {0}", text);
			while (i <= num3)
			{
				try
				{
					text2 = string.Empty;
					using (StreamReader streamReader = new StreamReader(text))
					{
						int partnerId = -1;
						int minorPartnerId = -1;
						int num8 = -1;
						Guid empty = Guid.Empty;
						string text4 = null;
						string text5;
						while ((text5 = streamReader.ReadLine()) != null)
						{
							if (text4 != null)
							{
								num2++;
								ExTraceGlobals.ServiceTracer.TraceWarning<string>(0L, "Could not parse line {0}", text4);
							}
							num++;
							text4 = text5;
							string[] array2 = text5.Split(new char[]
							{
								','
							});
							string text6;
							if ((array2.Length == 5 || array2.Length == 6) && GlobalLocatorCache.TryValidateAndGetString(array2[0], out text6))
							{
								if (text6.Equals("734ee9c9-4d44-4c05-b54e-0d64e642f672", StringComparison.OrdinalIgnoreCase))
								{
									int num9;
									if (!GlobalLocatorCache.TryValidateAndGetForestId(array2[1], out num9))
									{
										continue;
									}
									num5 = num9 / 16;
									if (GlobalLocatorCache.cacheLoadType == CacheLoadType.Local)
									{
										if (num6 > 0)
										{
											num5 /= num6;
										}
										else
										{
											num5 /= 35;
										}
									}
									else if (GlobalLocatorCache.cacheLoadType == CacheLoadType.Regional)
									{
										if (dictionary.Count > 0)
										{
											num7 = GlobalLocatorCache.GetRegionCountFromForestMap(dictionary);
										}
										num5 /= num7;
									}
								}
								else if (text6.Equals("b4d065b2-c71b-458b-b38a-08fea11efb00", StringComparison.OrdinalIgnoreCase))
								{
									string text7;
									int key;
									if (!GlobalLocatorCache.TryValidateAndGetString(array2[1], out text7) || !GlobalLocatorCache.TryValidateAndGetForestId(array2[2], out key))
									{
										continue;
									}
									if (!text7.ToLower().Contains("a00"))
									{
										num6++;
									}
									dictionary[key] = new ForestRegionTuple(text7);
								}
								else
								{
									int num10;
									if (!GlobalLocatorCache.TryValidateAndGetPartnerId(array2[1], out partnerId) || !GlobalLocatorCache.TryValidateAndGetPartnerId(array2[2], out minorPartnerId) || !GlobalLocatorCache.TryValidateAndGetForestId(array2[3], out num10))
									{
										continue;
									}
									int accountForestId;
									if (array2[4].Equals("\"\""))
									{
										num8 = num10;
										accountForestId = num10;
									}
									else
									{
										accountForestId = num10;
										if (!GlobalLocatorCache.TryValidateAndGetForestId(array2[4], out num8))
										{
											continue;
										}
									}
									if (!GlobalLocatorCache.ShouldLoadCacheEntry(num8, dictionary))
									{
										text4 = null;
										continue;
									}
									if (array2.Length == 6 && !GlobalLocatorCache.TryValidateAndGetTenantId(array2[5], out empty))
									{
										continue;
									}
									OfflineTenantInfo offlineTenantInfo = new OfflineTenantInfo(empty, partnerId, minorPartnerId, num8, accountForestId);
									Guid key2 = new Guid(text6);
									byte b = (byte)text6[0];
									Dictionary<Guid, OfflineTenantInfo> dictionary2 = array[(int)b];
									if (dictionary2 == null)
									{
										dictionary2 = new Dictionary<Guid, OfflineTenantInfo>(num5);
									}
									if (dictionary2.ContainsKey(key2))
									{
										ExTraceGlobals.ServiceTracer.TraceDebug(0L, "The domain hash {0} was already present in this file. Re-loading the entry PartnerId = {1}, MinorPartnerId = {2}, ResourceForest = {2}, AccountForest = {3}, TenantId = {4}", new object[]
										{
											text6,
											offlineTenantInfo.PartnerId,
											offlineTenantInfo.MinorPartnerId,
											offlineTenantInfo.ResourceForestId,
											offlineTenantInfo.AccountForestId,
											offlineTenantInfo.TenantId
										});
										dictionary2.Remove(key2);
										num4 -= 1L;
									}
									dictionary2.Add(key2, offlineTenantInfo);
									num4 += 1L;
									array[(int)b] = dictionary2;
								}
								text4 = null;
							}
						}
					}
				}
				catch (Exception ex)
				{
					i++;
					text3 = string.Format("Failed to load Global Locator cache from file {0} with {1} attempt(s)", text, i);
					text2 = ex.ToString();
					ExTraceGlobals.ServiceTracer.TraceError<string, string>(0L, "{0}. Exception : {1}", text3, text2);
					GlobalLocatorCache.eventLogger.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheLoadFailed, "GlobalLocatorCacheLoadFailed", new object[]
					{
						i,
						text,
						cacheLoaderName,
						text2
					});
					Thread.Sleep(10000);
					continue;
				}
				lock (GlobalLocatorCache.dictionaryLockObject)
				{
					GlobalLocatorCache.offlineTenantMapBuckets = array;
					GlobalLocatorCache.forestMap = dictionary;
					text3 = string.Format("Global Locator Cache was successfully loaded, total lines processed {0}, Cache size {1}", num, num4);
					GlobalLocatorCache.eventLogger.LogEvent(MSExchangeGlobalLocatorCacheEventLogConstants.Tuple_GlobalLocatorCacheLoaded, "GlobalLocatorCacheLoaded", new object[]
					{
						text,
						num,
						num2,
						num4,
						GlobalLocatorCache.cacheLoadType.ToString(),
						cacheLoaderName
					});
					ExTraceGlobals.ServiceTracer.TraceDebug(0L, text3);
					break;
				}
			}
			int ticksElapsed = Environment.TickCount - tickCount;
			GlobalLocatorCache.LogGlsMserv("LoadOfflineGlobalLocatorFile", text3, string.IsNullOrEmpty(text2), ticksElapsed, text2);
		}

		private static void LogGlsMserv(string methodName, string logMsg, bool isSuccess, int ticksElapsed, string failure)
		{
			string resultCode = isSuccess ? "Success" : "Failure";
			GLSLogger.BeginAppend(methodName, string.Empty, IPAddress.Loopback.ToString(), resultCode, null, (long)ticksElapsed, failure, "d36440f8-2eca-43f6-8c00-31824c99f89d", string.Empty, logMsg);
			GlobalLocatorCache.LogMserveInfo(logMsg, failure, ticksElapsed, methodName, string.Empty);
			GLSLogger.BeginAppend(methodName, logMsg, "localhost", resultCode, null, (long)ticksElapsed, failure, string.Empty, string.Empty, string.Empty);
		}

		private static string GetExchangeBinPath()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey == null)
				{
					result = string.Empty;
				}
				else
				{
					object value = registryKey.GetValue("MsiInstallPath");
					registryKey.Close();
					if (value == null)
					{
						result = string.Empty;
					}
					else
					{
						result = Path.Combine(value.ToString(), "Bin");
					}
				}
			}
			return result;
		}

		private static bool TryValidateAndGetString(string fragment, out string value)
		{
			value = string.Empty;
			if (!GlobalLocatorCache.IsValidFragment(fragment))
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The domain hash fragment {0} found in data file is not in correct format.", fragment);
				return false;
			}
			value = fragment.Substring(1, fragment.Length - 2);
			return true;
		}

		private static bool TryValidateAndGetPartnerId(string fragment, out int partnerId)
		{
			partnerId = -1;
			if (!GlobalLocatorCache.IsValidFragment(fragment))
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The partnerId/minorPartnerId fragment {0} found in data file is not in correct format.", fragment);
				return false;
			}
			string s = fragment.Substring(1, fragment.Length - 2);
			int num;
			if (int.TryParse(s, out num) && num >= 50000 && num <= 59999)
			{
				partnerId = num;
				return true;
			}
			if (num == -1)
			{
				partnerId = num;
				return true;
			}
			ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The partnerId/minorPartnerId {0} found in data file is invalid.", fragment);
			return false;
		}

		private static bool TryValidateAndGetTenantId(string fragment, out Guid tenantId)
		{
			tenantId = Guid.Empty;
			if (fragment.Equals("\"\""))
			{
				return true;
			}
			if (!GlobalLocatorCache.IsValidFragment(fragment))
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The tenantId fragment {0} found in data file is not in correct format.", fragment);
				return false;
			}
			string input = fragment.Substring(1, fragment.Length - 2);
			Guid guid;
			if (!Guid.TryParse(input, out guid))
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The tenantId {0} found in data file is invalid.", fragment);
				return false;
			}
			tenantId = guid;
			return true;
		}

		private static bool TryValidateAndGetForestId(string fragment, out int forestId)
		{
			forestId = -1;
			if (!GlobalLocatorCache.IsValidFragment(fragment))
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The RF/AF Id fragment {0} found in data file is not in correct format.", fragment);
				return false;
			}
			string s = fragment.Substring(1, fragment.Length - 2);
			int num;
			if (!int.TryParse(s, out num))
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "The RF/AF Id {0} found in data file is invalid.", fragment);
				return false;
			}
			forestId = num;
			return true;
		}

		private static bool IsValidFragment(string fragment)
		{
			return !string.IsNullOrEmpty(fragment) && fragment.Length >= 3 && fragment[0] == '"' && fragment[fragment.Length - 1] == '"';
		}

		private static bool ShouldLoadCacheEntry(int forestHash, Dictionary<int, ForestRegionTuple> forestMap)
		{
			CacheLoadType? cacheLoadType = GlobalLocatorCache.cacheLoadType;
			CacheLoadType valueOrDefault = cacheLoadType.GetValueOrDefault();
			if (cacheLoadType != null)
			{
				switch (valueOrDefault)
				{
				case CacheLoadType.Full:
					return true;
				case CacheLoadType.Regional:
					if (forestMap.ContainsKey(forestHash))
					{
						ForestRegionTuple forestRegionTuple = forestMap[forestHash];
						return forestRegionTuple.Region == GlobalLocatorCache.LocalForestRegion;
					}
					return false;
				case CacheLoadType.Local:
					return forestHash == GlobalLocatorCache.LocalForestHash;
				}
			}
			return false;
		}

		private static int GetRegionCountFromForestMap(Dictionary<int, ForestRegionTuple> forestMap)
		{
			List<string> list = new List<string>();
			foreach (ForestRegionTuple forestRegionTuple in forestMap.Values)
			{
				if (!list.Contains(forestRegionTuple.Region))
				{
					list.Add(forestRegionTuple.Region);
				}
			}
			return list.Count;
		}

		private CacheLoadType GetCacheLoadType()
		{
			try
			{
				if (ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("OverrideGlsCacheLoadType"))
				{
					return CacheLoadType.Full;
				}
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 811, "GetCacheLoadType", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\GlobalLocatorCache\\Program\\GlobalLocatorCache.cs");
				ServerRole currentServerRole = topologyConfigurationSession.FindLocalServer().CurrentServerRole;
				return ((currentServerRole & ServerRole.Cafe) == ServerRole.Cafe) ? CacheLoadType.Full : CacheLoadType.Local;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ServiceTracer.TraceError<string>(0L, "Got exception while calculating CacheLoadType. Exception: {0}", ex.ToString());
			}
			return CacheLoadType.Full;
		}

		public const string EventSourceName = "MSExchangeGlobalLocatorCache";

		internal const string GlsMetadataKey = "b4d065b2-c71b-458b-b38a-08fea11efb00";

		private const string GlobalLocatorCacheInfoRecordTrackingGuid = "d36440f8-2eca-43f6-8c00-31824c99f89d";

		private const string TotalLineCountKey = "734ee9c9-4d44-4c05-b54e-0d64e642f672";

		private const int CacheLoadIntervalMsec = 86400000;

		private const int SleepTimeBeforeCacheLoad = 10000;

		private const int InvalidPartnerId = -1;

		private const int InvalidForestId = -1;

		private static readonly int LocalForestHash = NativeHelpers.GetForestName().GetHashCode();

		private static readonly string LocalForestRegion = ForestRegionTuple.GetRegionFromForestFqdn(NativeHelpers.GetForestName());

		private static readonly string GlobalLocatorCacheFile = "OfflineGlobalLocatorServiceData.csv";

		private static readonly string prefixDomainEntryAddressFormatMinorPartnerId = "7f66cd009b304aeda37ffdeea1733ff6@{0}".Split(new char[]
		{
			'@'
		})[0];

		private static readonly string prefixDomainEntryAddressFormatMinorPartnerIdForOrgGuid = "3da19c7b44a74bd3896daaf008594b6c@{0}.exchangereserved".Split(new char[]
		{
			'@'
		})[0];

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ServiceTracer.Category, "MSExchangeGlobalLocatorCache");

		private static Dictionary<Guid, OfflineTenantInfo>[] offlineTenantMapBuckets = new Dictionary<Guid, OfflineTenantInfo>[256];

		private static Dictionary<int, ForestRegionTuple> forestMap;

		private static object dictionaryLockObject = new object();

		private static FileHandler fileHandler;

		private static Timer internalTimer;

		private static CacheLoadType? cacheLoadType = null;
	}
}
