using System;
using System.IO;
using Microsoft.Exchange.LogUploaderProxy;
using Microsoft.Win32;

namespace Microsoft.Exchange.LogUploader
{
	internal class Tools
	{
		public static int GetRegistryKeyIntValue(string key, string name)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("key", key);
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			int registryKeyIntValue;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key))
			{
				registryKeyIntValue = Tools.GetRegistryKeyIntValue(registryKey, name);
			}
			return registryKeyIntValue;
		}

		public static int GetRegistryKeyIntValue(RegistryKey key, string name)
		{
			int result = 0;
			if (key != null)
			{
				object value = key.GetValue(name);
				if (value != null)
				{
					result = (int)value;
				}
			}
			return result;
		}

		public static int RandomizeTimeSpan(TimeSpan baseTimeSpan, TimeSpan randomRange)
		{
			int num = (int)randomRange.TotalMilliseconds;
			if (num > 2147483647)
			{
				throw new ArgumentException(string.Format("The randomRange {0} exceeds Int32.Max when converting to milliseconds and can't be used as a random number range", num));
			}
			return (int)baseTimeSpan.TotalMilliseconds + new Random().Next(num);
		}

		public static bool IsRawProcessingType<T>() where T : LogDataBatch
		{
			return LogDataBatchReflectionCache<T>.IsRawBatch;
		}

		public static void GetPartitionFromPrefix(string fullFilePath, string prefixBeforePartitionId, out int partitionId, out SplitLogType splitLogType)
		{
			partitionId = -1;
			splitLogType = SplitLogType.Older;
			if (string.IsNullOrWhiteSpace(fullFilePath) || string.IsNullOrWhiteSpace(prefixBeforePartitionId))
			{
				throw new ArgumentException("Null or empty or white space input");
			}
			string fileName = Path.GetFileName(fullFilePath);
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException(string.Format("{0} is not a valid file path", fullFilePath));
			}
			if (!fileName.StartsWith(prefixBeforePartitionId, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException(string.Format("file name {0} does not start with prefixBeforePartitionId {1}", fileName, prefixBeforePartitionId));
			}
			string[] array = fileName.Substring(prefixBeforePartitionId.Length).Split(new char[]
			{
				'_'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2 || !int.TryParse(array[0], out partitionId))
			{
				throw new Exception(string.Format("log file name is not in a expected format of {0}_[PartitionId]_[Newer|Older]_xxx", prefixBeforePartitionId));
			}
			bool flag = false;
			foreach (object obj in Enum.GetValues(typeof(SplitLogType)))
			{
				SplitLogType splitLogType2 = (SplitLogType)obj;
				if (string.Compare(array[1], splitLogType2.ToString(), true) == 0)
				{
					flag = true;
					splitLogType = splitLogType2;
					break;
				}
			}
			if (!flag)
			{
				throw new Exception(string.Format("log file name is not in a expected format of {0}_[PartitionId]_[Newer|Older]_xxx", prefixBeforePartitionId));
			}
		}

		public static void DebugAssert(bool condition, string message)
		{
		}

		public const string ServiceName = "MSMessageTracingClient";

		public const string InsufficientResourcesString = "Insufficient system resources exist to complete the requested service";

		public const string ServerRole = "ServerRole";

		public const string SysProbeLogPrefix = "SYSPRB";

		public const string ServiceEnabledRegKey = "System\\CurrentControlSet\\Services\\MSMessageTracingClient";

		public const string ServiceEnabledRegKeyFullPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Services\\MSMessageTracingClient";

		public const string ServiceEnabledRegValue = "ServiceEnabled";

		public const string OpticsEnabledRegValue = "OpticsEnabled";

		public const string OpticsEnabledRegKeyFullPath = "HKEY_LOCAL_MACHINE\\OpticsEnabled";

		public static readonly TimeSpan SleepCheckStopRequestInterval = TimeSpan.FromSeconds(5.0);
	}
}
