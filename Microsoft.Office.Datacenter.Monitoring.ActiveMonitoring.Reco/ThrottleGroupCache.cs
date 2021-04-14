using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class ThrottleGroupCache
	{
		private ThrottleGroupCache()
		{
		}

		internal static ThrottleGroupCache Instance
		{
			get
			{
				return ThrottleGroupCache.lazy.Value;
			}
		}

		internal void StartPeriodicTimer()
		{
			GlobalTunables tunables = Dependencies.ThrottleHelper.Tunables;
			if (this.periodicTimer == null)
			{
				lock (this.timerLock)
				{
					if (this.periodicTimer == null)
					{
						this.periodicTimer = new Timer(delegate(object o)
						{
							this.UpdateCache();
						}, null, tunables.ThrottleGroupCacheRefreshStartDelay, tunables.ThrottleGroupCacheRefreshFrequency);
					}
				}
			}
		}

		internal void StopTimer()
		{
			lock (this.timerLock)
			{
				if (this.periodicTimer != null)
				{
					this.periodicTimer.Change(-1, -1);
					this.periodicTimer.Dispose();
				}
			}
		}

		internal void AddGroup(string groupName)
		{
			if (!string.IsNullOrEmpty(groupName))
			{
				this.UpdateGroupMembership(groupName, false);
				this.StartPeriodicTimer();
			}
		}

		internal void AddServers(string[] servers)
		{
			if (servers != null)
			{
				this.UpdateServersInfo(servers, false);
				this.StartPeriodicTimer();
			}
		}

		internal string[] GetServersInGroup(string groupName, bool isForceRefresh)
		{
			if (string.IsNullOrEmpty(groupName))
			{
				return null;
			}
			string[] array = null;
			lock (this.locker)
			{
				this.groupServersMap.TryGetValue(groupName, out array);
				if (isForceRefresh || array == null)
				{
					this.UpdateGroupMembership(groupName, false);
					this.groupServersMap.TryGetValue(groupName, out array);
				}
			}
			return array;
		}

		internal bool IsAllServersInGroupSupportVersion(string groupName, int requiredVersion, out int totalServersInGroup, out int totalServersInCompatibleVersion, bool isForceRefresh = false)
		{
			totalServersInGroup = 0;
			totalServersInCompatibleVersion = 0;
			this.UpdateGroupMembership(groupName, false);
			string[] serversInGroup = this.GetServersInGroup(groupName, isForceRefresh);
			return this.IsAllServersSupportVersion(serversInGroup, requiredVersion, out totalServersInGroup, out totalServersInCompatibleVersion, isForceRefresh);
		}

		internal bool IsAllServersSupportVersion(string[] servers, int requiredVersion, out int totalServersInGroup, out int totalServersInCompatibleVersion, bool isForceRefresh = false)
		{
			bool result = false;
			totalServersInGroup = ((servers != null) ? servers.Length : 0);
			totalServersInCompatibleVersion = 0;
			lock (this.locker)
			{
				if (servers != null)
				{
					this.UpdateServersInfo(servers, isForceRefresh);
					foreach (string key in servers)
					{
						ThrottleGroupCache.ServerMiniInfo serverMiniInfo = null;
						this.serverInfoMap.TryGetValue(key, out serverMiniInfo);
						if (serverMiniInfo != null && serverMiniInfo.Version >= requiredVersion)
						{
							totalServersInCompatibleVersion++;
						}
					}
					if (totalServersInCompatibleVersion == totalServersInGroup)
					{
						result = true;
					}
				}
			}
			return result;
		}

		internal void ClearCacheForTesting()
		{
			this.groupServersMap.Clear();
			this.serverInfoMap.Clear();
		}

		private void UpdateCache()
		{
			try
			{
				if (Interlocked.Increment(ref this.timerCounter) <= 1)
				{
					this.UpdateCacheInternal();
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.timerCounter);
			}
		}

		private string[] ResolveGroupNameToServers(string groupName)
		{
			string[] result = null;
			try
			{
				result = Dependencies.ThrottleHelper.Settings.GetServersInGroup(groupName);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "ResolveGroupNameToServers:GetServersInGroup() failed for group {0} with error {1}", groupName, ex.ToString(), null, "ResolveGroupNameToServers", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\ThrottleGroupCache.cs", 296);
			}
			return result;
		}

		private void UpdateCacheInternal()
		{
			lock (this.locker)
			{
				string[] array = this.groupServersMap.Keys.ToArray<string>();
				foreach (string text in array)
				{
					string[] array3 = this.ResolveGroupNameToServers(text);
					if (array3 != null)
					{
						this.groupServersMap[text] = array3;
						foreach (string key in array3)
						{
							ThrottleGroupCache.ServerMiniInfo serverMiniInfo = null;
							if (!this.serverInfoMap.TryGetValue(key, out serverMiniInfo))
							{
								this.serverInfoMap[key] = null;
							}
						}
					}
				}
				string[] array5 = this.serverInfoMap.Keys.ToArray<string>();
				foreach (string server in array5)
				{
					this.UpdateSingleServer(server);
				}
			}
		}

		private void UpdateGroupMembership(string groupName, bool isForceRefresh = false)
		{
			if (groupName == null)
			{
				return;
			}
			lock (this.locker)
			{
				string[] array = null;
				this.groupServersMap.TryGetValue(groupName, out array);
				if (array == null || isForceRefresh)
				{
					array = this.ResolveGroupNameToServers(groupName);
					this.groupServersMap[groupName] = array;
					this.UpdateServersInfo(array, isForceRefresh);
				}
			}
		}

		private void UpdateServersInfo(string[] servers, bool isForceRefresh = false)
		{
			if (servers == null || servers.Length == 0)
			{
				return;
			}
			lock (this.locker)
			{
				if (servers != null)
				{
					foreach (string text in servers)
					{
						ThrottleGroupCache.ServerMiniInfo serverMiniInfo = null;
						bool flag2 = isForceRefresh;
						if (!this.serverInfoMap.TryGetValue(text, out serverMiniInfo))
						{
							this.serverInfoMap[text] = null;
							flag2 = true;
						}
						if (flag2)
						{
							this.UpdateSingleServer(text);
						}
					}
				}
			}
		}

		private void UpdateSingleServer(string server)
		{
			try
			{
				ThrottleGroupCache.ServerMiniInfo value = new ThrottleGroupCache.ServerMiniInfo
				{
					Version = Dependencies.ThrottleHelper.GetServerVersion(server)
				};
				this.serverInfoMap[server] = value;
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "UpdateSingleServer() failed for server {0} with error {1}", server, ex.ToString(), null, "UpdateSingleServer", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\ThrottleGroupCache.cs", 425);
			}
		}

		private static readonly Lazy<ThrottleGroupCache> lazy = new Lazy<ThrottleGroupCache>(() => new ThrottleGroupCache());

		private readonly object locker = new object();

		private readonly object timerLock = new object();

		private Dictionary<string, string[]> groupServersMap = new Dictionary<string, string[]>();

		private Dictionary<string, ThrottleGroupCache.ServerMiniInfo> serverInfoMap = new Dictionary<string, ThrottleGroupCache.ServerMiniInfo>();

		private Timer periodicTimer;

		private TracingContext traceContext = TracingContext.Default;

		private int timerCounter;

		internal class ServerMiniInfo
		{
			internal int Version { get; set; }
		}
	}
}
