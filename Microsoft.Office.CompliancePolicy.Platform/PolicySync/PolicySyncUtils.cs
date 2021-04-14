using System;
using System.Collections.Concurrent;
using System.ServiceModel;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal static class PolicySyncUtils
	{
		public static void DisposeWcfClientGracefully(ICommunicationObject client, ExecutionLog logProvider, bool skipDispose = false)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					if (client == null)
					{
						return;
					}
					bool flag = false;
					try
					{
						if (client.State != CommunicationState.Faulted)
						{
							client.Close();
							flag = true;
						}
					}
					catch (FaultException<PolicySyncTransientFault> faultException)
					{
						logProvider.LogOneEntry(PolicySyncUtils.clientComponentName, null, ExecutionLog.EventType.Warning, "Transient Exception: " + faultException.InnerException.ToString(), null);
					}
					catch (FaultException<PolicySyncPermanentFault> faultException2)
					{
						logProvider.LogOneEntry(PolicySyncUtils.clientComponentName, null, ExecutionLog.EventType.Warning, "Permanent Exception: " + faultException2.InnerException.ToString(), null);
					}
					finally
					{
						if (!flag)
						{
							client.Abort();
						}
						if (!skipDispose)
						{
							((IDisposable)client).Dispose();
						}
					}
				});
			}
			catch (GrayException ex)
			{
				logProvider.LogOneEntry(PolicySyncUtils.clientComponentName, null, ExecutionLog.EventType.Warning, "Grey Exception:" + ex.ToString(), null);
			}
		}

		public static bool Implies(bool a, bool b)
		{
			return (a && b) || !a;
		}

		public static V GetOrAddSafe<T, V>(this ConcurrentDictionary<T, Lazy<V>> dictionary, T key, Func<T, V> valueFactory)
		{
			Lazy<V> orAdd = dictionary.GetOrAdd(key, new Lazy<V>(() => valueFactory(key)));
			return orAdd.Value;
		}

		private static readonly string clientComponentName = "UnifiedPolicySyncAgent";

		public class ServiceProxyPoolErrorData
		{
			public ServiceProxyPoolErrorData()
			{
			}

			public ServiceProxyPoolErrorData(string periodicKey, string debugMessage, int numberOfRetries)
			{
				this.PeriodicKey = periodicKey;
				this.DebugMessage = debugMessage;
				this.NumberOfRetries = numberOfRetries;
			}

			public string PeriodicKey { get; set; }

			public string DebugMessage { get; set; }

			public int NumberOfRetries { get; set; }

			public static PolicySyncUtils.ServiceProxyPoolErrorData GetFromString(string data)
			{
				string[] array = data.Split(new char[]
				{
					';'
				});
				return new PolicySyncUtils.ServiceProxyPoolErrorData
				{
					PeriodicKey = array[0].Trim().Split(new char[]
					{
						':'
					})[1].Trim(),
					DebugMessage = array[1].Trim().Split(new char[]
					{
						':'
					})[1].Trim(),
					NumberOfRetries = int.Parse(array[2].Trim().Split(new char[]
					{
						':'
					})[1].Trim())
				};
			}

			public override string ToString()
			{
				return string.Format("PeriodicKey: {0}; DebugMessage: {1}; NumberOfRetries: {2}", this.PeriodicKey, this.DebugMessage, this.NumberOfRetries);
			}
		}
	}
}
