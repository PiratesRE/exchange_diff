using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	public class GroupStatusCollector
	{
		public GroupStatusCollector(string self, InstanceClientFactory instanceClientFactory, InstanceGroupConfig groupConfig, IDxStoreEventLogger eventLogger, double requiredSuccessPercent)
		{
			this.Initialize(self, instanceClientFactory, groupConfig, eventLogger, requiredSuccessPercent);
		}

		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.HealthCheckerTracer;
			}
		}

		public IDxStoreEventLogger EventLogger { get; set; }

		public GroupStatusInfo GroupStatusInfo { get; set; }

		public void Initialize(string self, InstanceClientFactory instanceClientFactory, InstanceGroupConfig groupConfig, IDxStoreEventLogger eventLogger, double requiredSuccessPercent)
		{
			this.EventLogger = eventLogger;
			this.self = self;
			this.instanceClientFactory = instanceClientFactory;
			this.GroupStatusInfo = new GroupStatusInfo();
			this.groupConfig = groupConfig;
			this.completionEvent = new ManualResetEvent(false);
			this.requiredSuccessPercent = requiredSuccessPercent;
		}

		public void Run(TimeSpan waitDuration)
		{
			GroupStatusCollector.Tracer.TraceDebug<string>((long)this.groupConfig.Identity.GetHashCode(), "{0}: Starting group status collection", this.groupConfig.Identity);
			Stopwatch stopwatch = new Stopwatch();
			bool flag = false;
			try
			{
				stopwatch.Start();
				this.GroupStatusInfo.CollectionStartTime = DateTimeOffset.Now;
				this.RunInternal(waitDuration);
				flag = true;
			}
			finally
			{
				this.GroupStatusInfo.CollectionDuration = stopwatch.Elapsed;
				this.GroupStatusInfo.Analyze(this.self, this.groupConfig);
				GroupStatusCollector.Tracer.TraceDebug<string, string, string>((long)this.groupConfig.Identity.GetHashCode(), "{0}: Group status collection {1} (Info: {2})", this.groupConfig.Identity, flag ? "success" : "failed", this.GroupStatusInfo.GetDebugString(this.groupConfig.Identity));
			}
		}

		private void RunInternal(TimeSpan waitDuration)
		{
			GroupStatusInfo groupStatusInfo = this.GroupStatusInfo;
			groupStatusInfo.TotalRequested = this.groupConfig.Members.Length;
			InstanceGroupMemberConfig[] members = this.groupConfig.Members;
			for (int i = 0; i < members.Length; i++)
			{
				InstanceGroupMemberConfig instanceGroupMemberConfig = members[i];
				string memberName = instanceGroupMemberConfig.Name;
				groupStatusInfo.StatusMap[memberName] = null;
				Task.Factory.StartNew(delegate()
				{
					this.CollectStatusForServer(memberName, waitDuration);
				});
			}
			try
			{
				if (this.completionEvent.WaitOne(waitDuration) && groupStatusInfo.TotalNoReplies > 0)
				{
					TimeSpan timeSpan = DateTimeOffset.Now - groupStatusInfo.CollectionStartTime + waitDuration;
					if (timeSpan.TotalMilliseconds > 0.0)
					{
						int millisecondsTimeout = Math.Min((int)timeSpan.TotalMilliseconds, 2000);
						Thread.Sleep(millisecondsTimeout);
					}
				}
			}
			finally
			{
				this.MarkCompletion();
			}
		}

		private void MarkCompletion()
		{
			lock (this.locker)
			{
				this.isCompleted = true;
				if (this.completionEvent != null)
				{
					this.completionEvent.Dispose();
					this.completionEvent = null;
				}
			}
		}

		private void CollectStatusForServer(string serverName, TimeSpan waitDuration)
		{
			InstanceStatusInfo statusInfo = null;
			Exception ex = null;
			string label = string.Format("GetStatus({0})", serverName);
			if (!this.groupConfig.Settings.IsUseHttpTransportForInstanceCommunication)
			{
				ex = Utils.RunOperation(this.groupConfig.Identity, label, delegate
				{
					DxStoreInstanceClient client = this.instanceClientFactory.GetClient(serverName);
					statusInfo = client.GetStatus(null);
				}, this.EventLogger, LogOptions.LogException | this.groupConfig.Settings.AdditionalLogOptions, true, null, new TimeSpan?(this.groupConfig.Settings.PeriodicExceptionLoggingDuration), null, null, null);
			}
			else
			{
				try
				{
					string memberNetworkAddress = this.groupConfig.GetMemberNetworkAddress(serverName);
					Task<InstanceStatusInfo> statusAsync = HttpClient.GetStatusAsync(memberNetworkAddress, serverName, this.groupConfig.Name, this.self);
					statusAsync.Wait(waitDuration);
					statusInfo = statusAsync.Result;
				}
				catch (Exception ex2)
				{
					ex = ex2;
					this.EventLogger.Log(DxEventSeverity.Error, 0, "http send for GetStatusAsync failed: {0}", new object[]
					{
						ex2.ToString()
					});
				}
			}
			this.UpdateStatus(serverName, statusInfo, ex);
		}

		private void UpdateStatus(string serverName, InstanceStatusInfo statusInfo, Exception ex)
		{
			lock (this.locker)
			{
				this.UpdateStatusInternal(serverName, statusInfo, ex);
			}
		}

		private void UpdateStatusInternal(string serverName, InstanceStatusInfo statusInfo, Exception ex)
		{
			GroupStatusInfo groupStatusInfo = this.GroupStatusInfo;
			if (!this.isCompleted)
			{
				groupStatusInfo.StatusMap[serverName] = statusInfo;
				if (ex == null)
				{
					groupStatusInfo.TotalSuccessful++;
				}
				else
				{
					groupStatusInfo.TotalFailed++;
				}
				if (!this.isSignaled)
				{
					if (this.completionEvent != null)
					{
						double num = (double)groupStatusInfo.TotalSuccessful * 100.0 / (double)groupStatusInfo.TotalRequested;
						if (groupStatusInfo.TotalRequested == groupStatusInfo.TotalSuccessful + groupStatusInfo.TotalFailed || num >= this.requiredSuccessPercent)
						{
							GroupStatusCollector.Tracer.TraceDebug((long)this.groupConfig.Identity.GetHashCode(), "{0}: Signaling completion (Total: {1}, Success: {2}, Failed: {3}", new object[]
							{
								this.groupConfig.Identity,
								groupStatusInfo.TotalRequested,
								groupStatusInfo.TotalSuccessful,
								groupStatusInfo.TotalFailed
							});
							this.completionEvent.Set();
							this.isSignaled = true;
							return;
						}
					}
				}
				else
				{
					GroupStatusCollector.Tracer.TraceDebug((long)this.groupConfig.Identity.GetHashCode(), "{0}: Already signalled but trying level best to receive max replies in the grace time (Total: {1}, Success: {2}, Failed: {3}", new object[]
					{
						this.groupConfig.Identity,
						groupStatusInfo.TotalRequested,
						groupStatusInfo.TotalSuccessful,
						groupStatusInfo.TotalFailed
					});
				}
			}
		}

		private readonly object locker = new object();

		private InstanceGroupConfig groupConfig;

		private double requiredSuccessPercent;

		private string self;

		private bool isCompleted;

		private bool isSignaled;

		private ManualResetEvent completionEvent;

		private InstanceClientFactory instanceClientFactory;
	}
}
