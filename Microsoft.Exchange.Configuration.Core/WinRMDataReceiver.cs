using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Core
{
	internal class WinRMDataReceiver : WinRMDataExchanger
	{
		public WinRMDataReceiver(string connectionUrl, string userName, string authenticationType, LatencyTracker latencyTracker)
		{
			WinRMDataReceiver <>4__this = this;
			if (!WinRMDataExchangeHelper.IsExchangeDataUseAuthenticationType() && !WinRMDataExchangeHelper.IsExchangeDataUseNamedPipe())
			{
				throw new InvalidFlightingException();
			}
			CoreLogger.ExecuteAndLog("WinRMDataReceiver.Ctor", true, latencyTracker, delegate(Exception ex)
			{
				AuthZLogger.SafeAppendGenericError("WinRMDataReceiver.Ctor", ex.ToString(), false);
			}, delegate()
			{
				<>4__this.latencyTracker = latencyTracker;
				if (WinRMDataExchangeHelper.IsExchangeDataUseAuthenticationType())
				{
					WinRMDataExchangeHelper.DehydrateAuthenticationType(authenticationType, out <>4__this.authenticationType, out <>4__this.serializedData);
				}
				else
				{
					<>4__this.authenticationType = authenticationType;
				}
				string winRMDataIdentity = WinRMDataExchangeHelper.GetWinRMDataIdentity(connectionUrl, userName, <>4__this.authenticationType);
				CoreLogger.TraceDebug("[WinRMDataReceiver.Ctor]Initialize identity {0} with UseAuthenticationType: {1}, UseNamedPipe: {2}", new object[]
				{
					winRMDataIdentity,
					WinRMDataExchangeHelper.IsExchangeDataUseAuthenticationType(),
					WinRMDataExchangeHelper.IsExchangeDataUseNamedPipe()
				});
				Dictionary<string, string> dictionary = null;
				if (WinRMDataExchangeHelper.IsExchangeDataUseNamedPipe())
				{
					dictionary = <>4__this.LoadItemsFromNamedPipe(winRMDataIdentity);
					if (dictionary != null)
					{
						CoreLogger.TraceDebug("[WinRMDataReceiver.Ctor]LoadItemsFromNamedPipe for ideneity {0} successfully.", new object[]
						{
							winRMDataIdentity
						});
						AuthZLogger.SafeAppendGenericInfo("WinRMDataReceiver.NamedPipe", "Success");
					}
					else
					{
						CoreLogger.TraceDebug("[WinRMDataReceiver.Ctor]LoadItemsFromNamedPipe for ideneity {0} failed.", new object[]
						{
							winRMDataIdentity
						});
						AuthZLogger.SafeAppendGenericInfo("WinRMDataReceiver.NamedPipe", "Fail");
					}
				}
				if (dictionary == null && WinRMDataExchangeHelper.IsExchangeDataUseAuthenticationType())
				{
					dictionary = <>4__this.LoadItemsFromAuthenticationType(winRMDataIdentity);
					if (dictionary != null)
					{
						CoreLogger.TraceDebug("[WinRMDataReceiver.Ctor]LoadItemsFromAuthenticationType for ideneity {0} successfully.", new object[]
						{
							winRMDataIdentity
						});
						AuthZLogger.SafeAppendGenericInfo("WinRMDataReceiver.AuthenticationType", "Success");
					}
					else
					{
						CoreLogger.TraceDebug("[WinRMDataReceiver.Ctor]LoadItemsFromAuthenticationType for ideneity {0} failed.", new object[]
						{
							winRMDataIdentity
						});
						AuthZLogger.SafeAppendGenericInfo("WinRMDataReceiver.AuthenticationType", "Fail");
					}
				}
				if (dictionary == null)
				{
					throw new FailedToRecieveWinRMDataException(winRMDataIdentity);
				}
				foreach (string key in dictionary.Keys)
				{
					<>4__this.Items[key] = dictionary[key];
				}
				<>4__this.Identity = winRMDataIdentity;
			});
		}

		public string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

		private static void HandledExceptionFromCache(Exception e)
		{
			CoreLogger.LogEvent(TaskEventLogConstants.Tuple_WinRMDataReceiverHandledExceptionFromCache, null, new object[]
			{
				e.ToString()
			});
		}

		private Dictionary<string, string> LoadItemsFromNamedPipe(string identity)
		{
			return CoreLogger.ExecuteAndLog<Dictionary<string, string>>("WinRMDataReceiver.LoadItemsFromNamedPipe", false, this.latencyTracker, delegate(Exception ex)
			{
				AuthZLogger.SafeAppendGenericError("WinRMDataReceiver.LoadItemsFromNamedPipe", ex.ToString(), false);
			}, null, delegate()
			{
				IEnumerable<string> enumerable = WinRMDataReceiver.passiveObjectBehavior.RecieveMessages();
				if (enumerable != null)
				{
					foreach (string text in enumerable)
					{
						Dictionary<string, string> dictionary = WinRMDataExchangeHelper.DeSerialize(text);
						if (!dictionary.ContainsKey(WinRMDataExchanger.ItemKeyIdentity))
						{
							CoreLogger.TraceDebug("[WinRMDataReceiver.LoadItemsFromNamedPipe]LoadItemsFromNamedPipe can't find identity for message {0}.", new object[]
							{
								text
							});
							AuthZLogger.SafeAppendGenericError("WinRMDataReceiver.LoadItemsFromNamedPipe", string.Format("LoadItemsFromNamedPipe can't find identity for message {0}.", text), false);
						}
						else
						{
							WinRMDataReceiver.dataCache.TryInsertSliding(dictionary[WinRMDataExchanger.ItemKeyIdentity], dictionary, WinRMDataReceiver.MaxLiveTimeInDataCache);
						}
					}
				}
				Dictionary<string, string> result;
				if (WinRMDataReceiver.dataCache.TryGetValue(identity, out result))
				{
					return result;
				}
				return null;
			});
		}

		private Dictionary<string, string> LoadItemsFromAuthenticationType(string identity)
		{
			return CoreLogger.ExecuteAndLog<Dictionary<string, string>>("WinRMDataReceiver.LoadItemsFromAuthenticationType", false, this.latencyTracker, delegate(Exception ex)
			{
				AuthZLogger.SafeAppendGenericError("WinRMDataReceiver.LoadItemsFromAuthenticationType", ex.ToString(), false);
			}, null, () => WinRMDataExchangeHelper.DeSerialize(this.serializedData));
		}

		private static readonly int MaxItemsInDataCache = 50000;

		private static readonly TimeSpan MaxLiveTimeInDataCache = TimeSpan.FromMinutes(5.0);

		private static ExactTimeoutCache<string, Dictionary<string, string>> dataCache = new ExactTimeoutCache<string, Dictionary<string, string>>(null, null, new UnhandledExceptionDelegate(WinRMDataReceiver.HandledExceptionFromCache), WinRMDataReceiver.MaxItemsInDataCache, false);

		private static CrossAppDomainPassiveObjectBehavior passiveObjectBehavior = new CrossAppDomainPassiveObjectBehavior(WinRMDataExchanger.PipeName, BehaviorDirection.In);

		private string authenticationType;

		private string serializedData;

		private LatencyTracker latencyTracker;
	}
}
