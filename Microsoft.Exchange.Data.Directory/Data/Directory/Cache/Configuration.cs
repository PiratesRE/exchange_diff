using System;
using System.Diagnostics;
using System.Runtime.Caching;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal class Configuration
	{
		internal static bool IsCacheEnabled(string processNameOrProcessAppName)
		{
			bool result;
			try
			{
				result = Configuration.hookableInstance.Value.IsCacheEnabled(processNameOrProcessAppName);
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, processNameOrProcessAppName, new object[]
				{
					ex.ToString()
				});
				result = false;
			}
			return result;
		}

		internal static CacheMode GetCacheMode(string processNameOrProcessAppName)
		{
			CacheMode result;
			try
			{
				result = Configuration.hookableInstance.Value.GetCacheMode(processNameOrProcessAppName);
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, processNameOrProcessAppName, new object[]
				{
					ex.ToString()
				});
				result = CacheMode.Disabled;
			}
			return result;
		}

		internal static CacheMode GetCacheModeForCurrentProcess()
		{
			CacheMode result;
			try
			{
				result = Configuration.hookableInstance.Value.GetCacheModeForCurrentProcess();
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, Globals.ProcessName, new object[]
				{
					ex.ToString()
				});
				result = CacheMode.Disabled;
			}
			return result;
		}

		internal static bool IsCacheEnableForCurrentProcess()
		{
			bool result;
			try
			{
				result = Configuration.hookableInstance.Value.IsCacheEnableForCurrentProcess();
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, Globals.ProcessName, new object[]
				{
					ex.ToString()
				});
				result = false;
			}
			return result;
		}

		internal static bool IsCacheEnabled(Type type)
		{
			bool result;
			try
			{
				result = Configuration.hookableInstance.Value.IsCacheEnabled(type);
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, Globals.ProcessName, new object[]
				{
					ex.ToString()
				});
				result = false;
			}
			return result;
		}

		internal static bool IsCacheEnabledForInsertOnSave(ADRawEntry rawEntry)
		{
			bool result;
			try
			{
				result = Configuration.hookableInstance.Value.IsCacheEnabledForInsertOnSave(rawEntry);
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, Globals.ProcessName, new object[]
				{
					ex.ToString()
				});
				result = false;
			}
			return result;
		}

		internal static int GetCacheExpirationForObject(ADRawEntry rawEntry)
		{
			int result;
			try
			{
				result = Configuration.hookableInstance.Value.GetCacheExpirationForObject(rawEntry);
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, Globals.ProcessName, new object[]
				{
					ex.ToString()
				});
				result = 2147483646;
			}
			return result;
		}

		internal static CacheItemPriority GetCachePriorityForObject(ADRawEntry rawEntry)
		{
			CacheItemPriority result;
			try
			{
				result = Configuration.hookableInstance.Value.GetCachePriorityForObject(rawEntry);
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ReadADCacheConfigurationFailed, Globals.ProcessName, new object[]
				{
					ex.ToString()
				});
				result = CacheItemPriority.Default;
			}
			return result;
		}

		internal static IDisposable SetTestHook(ICacheConfiguration wrapper)
		{
			return Configuration.hookableInstance.SetTestHook(wrapper);
		}

		internal static void Refresh()
		{
			ConfigurationADImpl configurationADImpl = (ConfigurationADImpl)Configuration.hookableInstance.Value;
			Configuration.hookableInstance = Hookable<ICacheConfiguration>.Create(true, new ConfigurationADImpl());
			configurationADImpl.Dispose();
		}

		[Conditional("DEBUG")]
		internal static void FriendlyWatson(Exception exception)
		{
			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}
			ExWatson.SendReport(exception, ReportOptions.DeepStackTraceHash, null);
		}

		private static Hookable<ICacheConfiguration> hookableInstance = Hookable<ICacheConfiguration>.Create(true, new ConfigurationADImpl());
	}
}
