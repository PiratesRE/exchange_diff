using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class SharedHelper
	{
		internal static ExDateTime ExDateTimeMinValueUtc
		{
			get
			{
				return ExDateTime.MinValue.ToUtc();
			}
		}

		internal static DateTime DateTimeMinValueUtc
		{
			get
			{
				return DateTime.MinValue.ToUniversalTime();
			}
		}

		public static bool RunADOperation(EventHandler ev)
		{
			Exception ex = SharedHelper.RunADOperationEx(ev);
			if (ex != null)
			{
				AmTrace.Error("RunADOperation(): ADException occurred : {0}", new object[]
				{
					ex
				});
				return false;
			}
			return true;
		}

		public static Exception RunADOperationEx(EventHandler ev)
		{
			Exception result = null;
			try
			{
				ev(null, null);
			}
			catch (ADTopologyUnexpectedException ex)
			{
				result = ex;
			}
			catch (ADTopologyPermanentException ex2)
			{
				result = ex2;
			}
			catch (ADOperationException ex3)
			{
				result = ex3;
			}
			catch (ADTransientException ex4)
			{
				result = ex4;
			}
			catch (DataValidationException ex5)
			{
				result = ex5;
			}
			return result;
		}

		public static Exception RunClusterOperation(Action codeToRun)
		{
			Exception result = null;
			try
			{
				codeToRun();
			}
			catch (ClusterException ex)
			{
				result = ex;
			}
			return result;
		}

		internal static string GetFqdnNameFromNode(string nodeName)
		{
			if (string.IsNullOrEmpty(nodeName) || SharedHelper.StringIEquals(nodeName, "localhost"))
			{
				nodeName = Environment.MachineName;
			}
			if (!nodeName.Contains("."))
			{
				return AmServerNameCache.Instance.GetFqdn(nodeName);
			}
			return nodeName;
		}

		internal static bool IsLooksLikeFqdn(string serverName)
		{
			return MachineName.IsLikeFqdn(serverName);
		}

		internal static string GetNodeNameFromFqdn(string nodeName)
		{
			return MachineName.GetNodeNameFromFqdn(nodeName);
		}

		internal static bool StringIEquals(string str1, string str2)
		{
			return StringUtil.IsEqualIgnoreCase(str1, str2);
		}

		internal static int GetStringIHashCode(string str)
		{
			return StringUtil.GetStringIHashCode(str);
		}

		internal static void DisposeObjectList<T>(IEnumerable<T> objList) where T : IDisposable
		{
			if (objList != null)
			{
				foreach (T t in objList)
				{
					if (t != null)
					{
						t.Dispose();
					}
				}
			}
		}

		public static void SetRegistryProperty(string keyName, string propertyName, object propertyValue)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName, true))
			{
				if (registryKey != null)
				{
					registryKey.SetValue(propertyName, propertyValue);
				}
			}
		}

		public static T GetRegistryProperty<T>(string keyName, string propertyName, T defaultValue)
		{
			T result = defaultValue;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName, true))
			{
				if (registryKey != null)
				{
					result = (T)((object)registryKey.GetValue(propertyName, defaultValue));
				}
			}
			return result;
		}

		public static void Log(string format, params object[] args)
		{
			ReplayCrimsonEvents.GenericMessage.Log<string>(string.Format(format, args));
		}

		internal static string ExchangeKeyRoot = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		internal static string AmRegKeyRoot = SharedHelper.ExchangeKeyRoot + "\\ActiveManager";
	}
}
