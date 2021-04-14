using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class PswsBudgetManager : BudgetManager
	{
		private PswsBudgetManager()
		{
			int num;
			if (!int.TryParse(ConfigurationManager.AppSettings["RunspaceCacheTimeoutSec"], out num))
			{
				num = 600;
			}
			this.pswsRunspaceCacheTimeout = TimeSpan.FromSeconds((double)(num + 60));
			this.budgetTimeout = this.pswsRunspaceCacheTimeout.Add(TimeSpan.FromSeconds(10.0));
		}

		internal static PswsBudgetManager Instance
		{
			get
			{
				return PswsBudgetManager.instance;
			}
		}

		protected override TimeSpan BudgetTimeout
		{
			get
			{
				return this.budgetTimeout;
			}
		}

		internal void StartRunspace(AuthZPluginUserToken userToken)
		{
			string runspaceCacheKey = this.GetRunspaceCacheKey(userToken);
			if (string.IsNullOrEmpty(runspaceCacheKey))
			{
				AuthZLogger.SafeAppendGenericError("NullOrEmptyRunspaceCacheKey", "User token have an empty ExecutingUserName", false);
				return;
			}
			lock (base.InstanceLock)
			{
				RunspaceCacheValue runspaceCacheValue;
				if (this.runspaceCache.TryGetValue(runspaceCacheKey, out runspaceCacheValue))
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>((long)this.GetHashCode(), "[PswsBudgetManager.StartRunspace] item {0} is removed explicitly", runspaceCacheKey);
					if (runspaceCacheValue != null && runspaceCacheValue.CostHandle != null)
					{
						runspaceCacheValue.CostHandle.Dispose();
					}
					this.runspaceCache.Remove(runspaceCacheKey);
				}
				CostHandle costHandle = this.StartRunspaceImpl(userToken);
				RunspaceCacheValue value2 = new RunspaceCacheValue
				{
					CostHandle = costHandle,
					UserToken = (PswsAuthZUserToken)userToken
				};
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, TimeSpan>((long)this.GetHashCode(), "[PswsBudgetManager.StartRunspace] Add value {0} to runspace cache. Expired time = {1}.", runspaceCacheKey, this.pswsRunspaceCacheTimeout);
				this.runspaceCache.InsertAbsolute(runspaceCacheKey, value2, this.pswsRunspaceCacheTimeout, new RemoveItemDelegate<string, RunspaceCacheValue>(this.OnRunspaceCacheItemExpired));
			}
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>((long)this.GetHashCode(), "[PswsBudgetManager.StartRunspace] Add/Update value {0} to connectedUser cache.", runspaceCacheKey);
			this.connectedUsers.AddOrUpdate(runspaceCacheKey, ExDateTime.Now, (string key, ExDateTime value) => ExDateTime.Now);
			AuthZPluginHelper.UpdateAuthZPluginPerfCounters(this);
		}

		internal string GetConnectedUsers()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, ExDateTime> keyValuePair in this.connectedUsers)
			{
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append(',');
				stringBuilder.Append(keyValuePair.Value);
				stringBuilder.Append(',');
			}
			return stringBuilder.ToString();
		}

		protected override void HeartBeatImpl(AuthZPluginUserToken userToken)
		{
			base.HeartBeatImpl(userToken);
			string runspaceCacheKey = this.GetRunspaceCacheKey(userToken);
			lock (base.InstanceLock)
			{
				RunspaceCacheValue value;
				if (!this.runspaceCache.TryGetValue(runspaceCacheKey, out value))
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceError<string>((long)this.GetHashCode(), "[PswsBudgetManager.HeartBeatImpl] Unexpected: User {0} is not in the runspace cache in heart-beat.", runspaceCacheKey);
				}
				else
				{
					this.runspaceCache.InsertAbsolute(runspaceCacheKey, value, this.pswsRunspaceCacheTimeout, new RemoveItemDelegate<string, RunspaceCacheValue>(this.OnRunspaceCacheItemExpired));
				}
			}
		}

		private string GetRunspaceCacheKey(AuthZPluginUserToken userToken)
		{
			return ((PswsAuthZUserToken)userToken).ExecutingUserName;
		}

		private void OnRunspaceCacheItemExpired(string key, RunspaceCacheValue value, RemoveReason reason)
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, RemoveReason>((long)this.GetHashCode(), "[PswsBudgetManager.OnRunspaceCacheItemExpired] item {0} is removed. Reason = {1}", key, reason);
			if (reason != RemoveReason.Removed)
			{
				try
				{
					if (value != null)
					{
						if (value.CostHandle != null)
						{
							value.CostHandle.Dispose();
						}
						else
						{
							ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[PswsBudgetManager.OnRunspaceCacheItemExpired] value.CostHandle = null");
						}
						if (value.UserToken == null)
						{
							ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[PswsBudgetManager.OnRunspaceCacheItemExpired] value.UserToken = null");
						}
						else
						{
							base.RemoveBudgetIfNoActiveRunspace(value.UserToken);
							string runspaceCacheKey = this.GetRunspaceCacheKey(value.UserToken);
							ExDateTime exDateTime;
							if (runspaceCacheKey != null && this.connectedUsers.TryRemove(runspaceCacheKey, out exDateTime))
							{
								ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>((long)this.GetHashCode(), "[PswsBudgetManager.OnRunspaceCacheItemExpired] User {0} is removed from connectedUsers cache.", runspaceCacheKey);
							}
						}
					}
				}
				finally
				{
					AuthZPluginHelper.UpdateAuthZPluginPerfCounters(this);
				}
			}
		}

		private readonly TimeSpan pswsRunspaceCacheTimeout;

		private readonly TimeSpan budgetTimeout;

		private readonly TimeoutCache<string, RunspaceCacheValue> runspaceCache = new TimeoutCache<string, RunspaceCacheValue>(20, 5000, false);

		private readonly ConcurrentDictionary<string, ExDateTime> connectedUsers = new ConcurrentDictionary<string, ExDateTime>();

		private static readonly PswsBudgetManager instance = new PswsBudgetManager();
	}
}
