using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal static class Utility
	{
		public static void DoWorkAndLogIfFail(Action doWork, ExecutionLog logger, string tenantId, string correlationId, ExecutionLog.EventType eventType, string tag, string contextData, bool reThrowKnownException, bool reThrowGrayException = false)
		{
			try
			{
				ExecutionWrapper.DoRetriableWorkAndLogIfFail(doWork, 0U, (Exception ex) => ex is SyncAgentTransientException, logger, "UnifiedPolicySyncAgent", tenantId, correlationId, eventType, tag, contextData, true);
			}
			catch (GrayException ex)
			{
				GrayException ex2;
				logger.LogError("UnifiedPolicySyncAgent", tenantId, correlationId, ex2, contextData, new KeyValuePair<string, object>[0]);
				if (reThrowGrayException)
				{
					throw ex2;
				}
			}
			catch (SyncAgentExceptionBase syncAgentExceptionBase)
			{
				logger.LogError("UnifiedPolicySyncAgent", tenantId, correlationId, syncAgentExceptionBase, contextData, new KeyValuePair<string, object>[0]);
				if (reThrowKnownException)
				{
					throw syncAgentExceptionBase;
				}
			}
		}

		public static IDictionary<string, string> Merge(this IDictionary<string, string> currentDict, IDictionary<string, string> newDict)
		{
			ArgumentValidator.ThrowIfNull("newDict", newDict);
			foreach (KeyValuePair<string, string> item in newDict)
			{
				if (!currentDict.ContainsKey(item.Key))
				{
					currentDict.Add(item);
				}
			}
			return currentDict;
		}

		public static Dictionary<K, V> CreateDictionaryFromList<K, V>(List<KeyValuePair<K, V>> list)
		{
			ArgumentValidator.ThrowIfNull("list", list);
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			foreach (KeyValuePair<K, V> keyValuePair in list)
			{
				if (!dictionary.ContainsKey(keyValuePair.Key))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		public static string GetThreadPoolStatus()
		{
			int num;
			int num2;
			ThreadPool.GetMaxThreads(out num, out num2);
			int num3;
			int num4;
			ThreadPool.GetAvailableThreads(out num3, out num4);
			return string.Format("ThreadPool Status: max worker thread: {0}; available worker thread: {1}; max IO thread: {2}; available IO thread: {3}.", new object[]
			{
				num,
				num3,
				num2,
				num4
			});
		}

		public static object GetWholeProperty(object obj, string propertyName)
		{
			object result = null;
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
			PropertyInfo property = obj.GetType().GetProperty(propertyName, bindingAttr);
			if (property != null)
			{
				result = property.GetValue(obj);
			}
			return result;
		}

		public static bool GetIncrementalProperty(object obj, string propertyName, out object result)
		{
			bool result2 = false;
			result = null;
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
			PropertyInfo property = obj.GetType().GetProperty(propertyName, bindingAttr);
			if (property != null)
			{
				object value = property.GetValue(obj);
				if (value != null)
				{
					PropertyInfo property2 = value.GetType().GetProperty("Changed", bindingAttr);
					if (property2 != null && (bool)property2.GetValue(value))
					{
						result2 = true;
						result = value.GetType().GetProperty("Value", bindingAttr).GetValue(value);
					}
				}
			}
			return result2;
		}
	}
}
