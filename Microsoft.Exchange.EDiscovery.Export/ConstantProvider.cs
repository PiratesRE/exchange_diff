using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal static class ConstantProvider
	{
		static ConstantProvider()
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Exchange\\Client\\eDiscovery\\ExportTool"))
					{
						if (registryKey2 != null)
						{
							ConstantProvider.SearchMailboxesPageSize = ConstantProvider.GetValue<int>(registryKey2, "SearchMailboxesPageSize", 500, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.ExportBatchItemCountLimit = ConstantProvider.GetValue<int>(registryKey2, "ExportBatchItemCountLimit", 250, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.ExportBatchSizeLimit = ConstantProvider.GetValue<int>(registryKey2, "ExportBatchSizeLimit", 5242880, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.ItemIdListCacheSize = ConstantProvider.GetValue<int>(registryKey2, "ItemIdListCacheSize", 500, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.RetryInterval = ConstantProvider.GetValue<int>(registryKey2, "RetryInterval", 30000, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.AutoDiscoverBatchSize = ConstantProvider.GetValue<int>(registryKey2, "AutoDiscoverBatchSize", 50, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.maxCSVLogFileSizeInBytes = ConstantProvider.GetValue<int>(registryKey2, "MaxCSVLogFileSizeInBytes", 104857600, new ConstantProvider.TryParse<int>(int.TryParse));
							ConstantProvider.pstSizeLimitInBytes = ConstantProvider.GetValue<long>(registryKey2, "PSTSizeLimitInBytes", 10000000000L, new ConstantProvider.TryParse<long>(long.TryParse));
							ConstantProvider.TotalRetryTimeWindow = ConstantProvider.GetValue<TimeSpan>(registryKey2, "TotalRetryTimeWindow", DiscoveryConstants.DefaultTotalRetryTimeWindow, new ConstantProvider.TryParse<TimeSpan>(TimeSpan.TryParse));
							ConstantProvider.RetrySchedule = ConstantProvider.GetArrayValue<TimeSpan>(registryKey2, "RetrySchedule", DiscoveryConstants.DefaultRetrySchedule, new ConstantProvider.TryParse<TimeSpan>(TimeSpan.TryParse));
							ConstantProvider.RebindWithAutoDiscoveryEnabled = ConstantProvider.GetValue<bool>(registryKey2, "RebindWithAutoDiscoveryEnabled", false, new ConstantProvider.TryParse<bool>(bool.TryParse));
							ConstantProvider.RebindAutoDiscoveryUrl = null;
							ConstantProvider.RebindAutoDiscoveryInternalUrlOnly = true;
							ConstantProvider.SearchStatisticsEnabled = ConstantProvider.GetValue<bool>(registryKey2, "SearchStatisticsEnabled", false, new ConstantProvider.TryParse<bool>(bool.TryParse));
							int value = ConstantProvider.GetValue<int>(registryKey2, "partitionCSVLogFile", 0, new ConstantProvider.TryParse<int>(int.TryParse));
							if (value == 1)
							{
								ConstantProvider.partitionCSVLogFile = true;
							}
						}
						else
						{
							Tracer.TraceInformation("ConstantProvider..Ctor: Registry not found for constants", new object[0]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceError("ConstantProvider..Ctor: Failed to load registry data. Details: {0}", new object[]
				{
					ex
				});
			}
		}

		public static int SearchMailboxesPageSize
		{
			get
			{
				return ConstantProvider.searchMailboxesPageSize;
			}
			internal set
			{
				ConstantProvider.searchMailboxesPageSize = value;
			}
		}

		public static int ExportBatchItemCountLimit
		{
			get
			{
				return ConstantProvider.exportBatchItemCountLimit;
			}
			internal set
			{
				ConstantProvider.exportBatchItemCountLimit = value;
			}
		}

		public static int ExportBatchSizeLimit
		{
			get
			{
				return ConstantProvider.exportBatchSizeLimit;
			}
			internal set
			{
				ConstantProvider.exportBatchSizeLimit = value;
			}
		}

		public static int ItemIdListCacheSize
		{
			get
			{
				return ConstantProvider.itemIdListCacheSize;
			}
			internal set
			{
				ConstantProvider.itemIdListCacheSize = value;
			}
		}

		public static int RetryInterval
		{
			get
			{
				return ConstantProvider.retryInterval;
			}
			internal set
			{
				ConstantProvider.retryInterval = value;
			}
		}

		public static TimeSpan TotalRetryTimeWindow
		{
			get
			{
				return ConstantProvider.totalRetryTimeWindow;
			}
			internal set
			{
				ConstantProvider.totalRetryTimeWindow = value;
			}
		}

		public static TimeSpan[] RetrySchedule
		{
			get
			{
				return ConstantProvider.retrySchedule;
			}
			internal set
			{
				ConstantProvider.retrySchedule = value;
			}
		}

		public static int AutoDiscoverBatchSize
		{
			get
			{
				return ConstantProvider.autoDiscoverBatchSize;
			}
			internal set
			{
				ConstantProvider.autoDiscoverBatchSize = value;
			}
		}

		public static int MaxCSVLogFileSizeInBytes
		{
			get
			{
				return ConstantProvider.maxCSVLogFileSizeInBytes;
			}
			internal set
			{
				ConstantProvider.maxCSVLogFileSizeInBytes = value;
			}
		}

		public static long PSTSizeLimitInBytes
		{
			get
			{
				return ConstantProvider.pstSizeLimitInBytes;
			}
			internal set
			{
				ConstantProvider.pstSizeLimitInBytes = value;
			}
		}

		public static bool PartitionCSVLogFile
		{
			get
			{
				return ConstantProvider.partitionCSVLogFile;
			}
			internal set
			{
				ConstantProvider.partitionCSVLogFile = value;
			}
		}

		public static bool RebindWithAutoDiscoveryEnabled { get; set; }

		public static bool RebindAutoDiscoveryInternalUrlOnly { get; set; }

		public static Uri RebindAutoDiscoveryUrl { get; set; }

		public static bool SearchStatisticsEnabled { get; set; }

		private static T GetValue<T>(RegistryKey configuration, string name, T defaultValue, ConstantProvider.TryParse<T> tryParse)
		{
			string text = configuration.GetValue(name) as string;
			T result;
			if (text == null || !tryParse(text, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		private static T[] GetArrayValue<T>(RegistryKey configuration, string name, T[] defaultValue, ConstantProvider.TryParse<T> tryParse)
		{
			string[] array = configuration.GetValue(name) as string[];
			T[] array2;
			if (array == null || array.Length == 0)
			{
				array2 = defaultValue;
			}
			else
			{
				array2 = new T[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (!tryParse(array[i], out array2[i]))
					{
						array2 = defaultValue;
						break;
					}
				}
			}
			return array2;
		}

		private static int searchMailboxesPageSize = 500;

		private static int exportBatchItemCountLimit = 250;

		private static int exportBatchSizeLimit = 5242880;

		private static int itemIdListCacheSize = 500;

		private static int retryInterval = 30000;

		private static TimeSpan totalRetryTimeWindow = DiscoveryConstants.DefaultTotalRetryTimeWindow;

		private static TimeSpan[] retrySchedule = DiscoveryConstants.DefaultRetrySchedule;

		private static int autoDiscoverBatchSize = 50;

		private static int maxCSVLogFileSizeInBytes = 104857600;

		private static long pstSizeLimitInBytes = 10000000000L;

		private static bool partitionCSVLogFile = true;

		private delegate bool TryParse<T>(string config, out T parsedConfig);
	}
}
