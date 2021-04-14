using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ResourceManager
	{
		public ResourceManager(ResourceManagerConfiguration resourceManagerConfig, ResourceMonitorFactory resourceMonitorFactory, ResourceManagerEventLogger eventLogger, ResourceManagerComponentsAdapter componentsAdapter, ResourceManagerResources resourcesToMonitor, ResourceLog resourceLog)
		{
			ArgumentValidator.ThrowIfNull("resourceManagerConfig", resourceManagerConfig);
			ArgumentValidator.ThrowIfNull("resourceMonitorFactory", resourceMonitorFactory);
			ArgumentValidator.ThrowIfNull("eventLogger", eventLogger);
			ArgumentValidator.ThrowIfNull("componentsAdapter", componentsAdapter);
			ArgumentValidator.ThrowIfNull("resourceLog", resourceLog);
			this.components = componentsAdapter;
			this.resourcesToMonitor = resourcesToMonitor;
			this.resourceManagerConfig = resourceManagerConfig;
			this.resourceMonitorFactory = resourceMonitorFactory;
			this.eventLogger = eventLogger;
			this.resourceLog = resourceLog;
		}

		public bool IsMonitoringEnabled
		{
			get
			{
				return this.resourceManagerConfig.EnableResourceMonitoring;
			}
		}

		public TimeSpan MonitorInterval
		{
			get
			{
				return this.resourceManagerConfig.ResourceMonitoringInterval;
			}
		}

		public ResourceUses CurrentPrivateBytesUses
		{
			get
			{
				ResourceUses result;
				try
				{
					this.updateLock.AcquireReaderLock(-1);
					result = this.currentPrivateBytesUses;
				}
				finally
				{
					this.updateLock.ReleaseReaderLock();
				}
				return result;
			}
		}

		public DateTime LastTimeResourceMonitored
		{
			get
			{
				return this.lastTimeResourceMonitored;
			}
		}

		public bool ShouldShrinkDownMemoryCaches
		{
			get
			{
				return this.IsMonitoringEnabled && (this.resourceMonitors[3].CurrentResourceUsesRaw > ResourceUses.Normal || this.resourceMonitors[3].ResourceUses > ResourceUses.Normal || this.resourceMonitors[4].ResourceUses > ResourceUses.Normal);
			}
		}

		public void OnMonitorResource(object obj)
		{
			this.lastTimeResourceMonitored = DateTime.UtcNow;
			foreach (ResourceMonitor resourceMonitor in this.resourceMonitors)
			{
				resourceMonitor.UpdateReading();
			}
			ResourceMonitor resourceMonitor2 = this.resourceMonitors[3];
			if (resourceMonitor2.CurrentResourceUsesRaw > ResourceUses.Normal && !this.collectingGarbage)
			{
				bool flag = resourceMonitor2.PreviousResourceUses != resourceMonitor2.ResourceUses;
				DateTime utcNow = DateTime.UtcNow;
				if (flag || utcNow > this.lastTimeGCCollectCalled.Add(this.gcCollectInterval))
				{
					int num = GC.CollectionCount(2);
					if (flag || this.lastGCCollectionCount == -1 || num == this.lastGCCollectionCount)
					{
						this.collectingGarbage = true;
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.DoGarbageCollection));
					}
					else
					{
						this.lastGCCollectionCount = num;
					}
				}
			}
			ComponentsState componentsState = ComponentsState.AllowAllComponents;
			ResourceMonitor resourceMonitor3 = this.resourceMonitors[2];
			ResourceMonitor resourceMonitor4 = this.resourceMonitors[5];
			bool flag2 = false;
			if (resourceMonitor3.CurrentPressureRaw >= resourceMonitor3.LowPressureLimit || resourceMonitor4.CurrentPressureRaw >= resourceMonitor4.LowPressureLimit)
			{
				ExTraceGlobals.ResourceManagerTracer.TraceDebug(0L, "CurrentPressureRaw = {0}, CurrentPressureStabilized = {1}, ResourceUsesRaw = {2}, ResourceUsesStabilized = {3}.", new object[]
				{
					resourceMonitor3.CurrentPressureRaw,
					resourceMonitor3.CurrentPressure,
					resourceMonitor3.CurrentResourceUsesRaw,
					resourceMonitor3.ResourceUses
				});
				flag2 = true;
				componentsState &= ~ComponentsState.AllowContentAggregation;
			}
			if (flag2)
			{
				this.smtpThrottlingController.Increase();
			}
			else
			{
				this.smtpThrottlingController.Decrease();
			}
			if (this.components.SmtpInComponent != null)
			{
				TimeSpan current = this.smtpThrottlingController.GetCurrent();
				if (current == TimeSpan.Zero)
				{
					this.components.SmtpInComponent.SetThrottleDelay(current, null);
				}
				else
				{
					string throttleDelayContext = string.Format(CultureInfo.InvariantCulture, "VB={0};QS={1}", new object[]
					{
						resourceMonitor3.CurrentPressureRaw,
						resourceMonitor4.CurrentPressureRaw
					});
					this.components.SmtpInComponent.SetThrottleDelay(current, throttleDelayContext);
				}
			}
			foreach (ResourceMonitor resourceMonitor5 in this.resourceMonitors)
			{
				resourceMonitor5.DoCleanup();
			}
			this.PostMonitorResource(componentsState);
		}

		public void HintGCCollectCouldBeEffective()
		{
			this.gcCollectInterval = ResourceManager.gcCollectLowInterval;
		}

		public void Load()
		{
			if (this.IsMonitoringEnabled)
			{
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.MailDatabase) > ResourceManagerResources.None) ? this.resourceMonitorFactory.MailDatabaseMonitor : new NullResourceMonitor("NullMailDatabaseMonitor"));
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.MailDatabaseLoggingFolder) > ResourceManagerResources.None) ? this.resourceMonitorFactory.MailDatabaseLoggingFolderMonitor : new NullResourceMonitor("NullMailDatabaseLoggingFolderMonitor"));
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.VersionBuckets) > ResourceManagerResources.None) ? this.resourceMonitorFactory.VersionBucketResourceMonitor : new NullResourceMonitor("NullVersionBucketsMonitor"));
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.PrivateBytes) > ResourceManagerResources.None) ? this.resourceMonitorFactory.MemoryPrivateBytesMonitor : new NullResourceMonitor("NullPrivateBytesMonitor"));
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.TotalBytes) > ResourceManagerResources.None) ? this.resourceMonitorFactory.MemoryTotalBytesMonitor : new NullResourceMonitor("NullTotalBytesMonitor"));
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.SubmissionQueue) > ResourceManagerResources.None) ? this.resourceMonitorFactory.SubmissionQueueMonitor : new NullResourceMonitor("NullSubmissionQueueMonitor"));
				this.resourceMonitors.Add(((this.resourcesToMonitor & ResourceManagerResources.TempDrive) > ResourceManagerResources.None) ? this.resourceMonitorFactory.TempDriveMonitor : new NullResourceMonitor("NullSubmissionQueueMonitor"));
				foreach (ResourceMonitor resourceMonitor in this.resourceMonitors)
				{
					resourceMonitor.UpdateConfig();
				}
				this.smtpThrottlingController = new ThrottlingController(ExTraceGlobals.ResourceManagerTracer, this.resourceManagerConfig.ThrottlingControllerConfig);
				this.OnMonitorResource(null);
			}
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void RefreshComponentsState()
		{
			this.RefreshComponentsState(ComponentsState.AllowAllComponents);
		}

		public void RefreshComponentsState(ComponentsState requiredComponentsState)
		{
			if (this.IsMonitoringEnabled)
			{
				ResourceUses resourceUses;
				ResourceUses resourceUses2;
				ResourceUses resourceUses3;
				ResourceUses resourceUses4;
				try
				{
					this.updateLock.AcquireReaderLock(-1);
					resourceUses = this.currentResourceUses;
					resourceUses2 = this.currentPrivateBytesUses;
					resourceUses3 = this.currentVersionBucketUses;
					resourceUses4 = this.currentSubmissionQueueUses;
				}
				finally
				{
					this.updateLock.ReleaseReaderLock();
				}
				if (resourceUses == ResourceUses.High)
				{
					ExTraceGlobals.ResourceManagerTracer.TraceDebug(0L, "BackPressure: High: Disable SmtpIn (from organization and internet), Pickup, Replay, Aggregation, and Store Driver (inbound submission from mailbox)");
					requiredComponentsState &= ComponentsState.HighResourcePressureState;
				}
				else if (resourceUses == ResourceUses.Medium)
				{
					ExTraceGlobals.ResourceManagerTracer.TraceDebug(0L, "BackPressure: Medium: Disable incomding email from Internet but allow incoming email from Organization. Disable Pick, Replay, Aggregation. Enable Store Driver (inbound submission from mailbox).");
					requiredComponentsState &= ComponentsState.MediumResourcePressureState;
				}
				else
				{
					ExTraceGlobals.ResourceManagerTracer.TraceDebug(0L, "BackPressure: Normal: Enable SmtpIn from Internet and Organization, Pickup, Replay, Aggregation, and Store Driver (inbound submission from mailbox).");
				}
				if (resourceUses2 > ResourceUses.Normal && resourceUses2 > this.resourceMonitors[3].PreviousResourceUses && !this.components.ShuttingDown && this.components.IsActive)
				{
					lock (this.components.SyncRoot)
					{
						if (this.components.TransportIsMemberOfResolverComponent != null && !this.components.ShuttingDown && this.components.IsActive)
						{
							this.components.TransportIsMemberOfResolverComponent.ClearCache();
						}
					}
				}
				if (resourceUses2 > ResourceUses.Normal || resourceUses4 > ResourceUses.Normal)
				{
					requiredComponentsState &= ~ComponentsState.AllowBootScannerRunning;
				}
				if (resourceUses3 > ResourceUses.Normal)
				{
					requiredComponentsState &= ~ComponentsState.AllowOutboundMailDeliveryToRemoteDomains;
				}
			}
			this.components.UpdateComponentsState(requiredComponentsState);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			string text = this.components.ToString();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			foreach (ResourceMonitor resourceMonitor in this.resourceMonitors)
			{
				if (resourceMonitor.ResourceUses == ResourceUses.Normal)
				{
					stringBuilder2.AppendLine(resourceMonitor.ToString());
				}
				else
				{
					stringBuilder3.AppendLine(resourceMonitor.ToString());
				}
			}
			if (stringBuilder3.Length > 0)
			{
				stringBuilder.AppendLine(Strings.ResourcesInAboveNormalPressure(stringBuilder3.ToString()));
			}
			if (text.Length > 0)
			{
				stringBuilder.AppendLine(Strings.ComponentsDisabledByBackPressure(text));
			}
			else
			{
				stringBuilder.AppendLine(Strings.ComponentsDisabledNone);
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder.AppendLine(Strings.ResourcesInNormalPressure(stringBuilder2.ToString()));
			}
			return stringBuilder.ToString();
		}

		internal static string MapToLocalizedString(ResourceUses resourceUses)
		{
			string result = string.Empty;
			switch (resourceUses)
			{
			case ResourceUses.Normal:
				result = Strings.NormalResourceUses;
				break;
			case ResourceUses.Medium:
				result = Strings.MediumResourceUses;
				break;
			case ResourceUses.High:
				result = Strings.HighResourceUses;
				break;
			}
			return result;
		}

		internal void AddDiagnosticInfoTo(XElement resourceManagerElement, bool showBasic, bool showVerbose)
		{
			if (resourceManagerElement == null)
			{
				throw new ArgumentNullException("resourceManagerElement");
			}
			this.resourceManagerConfig.AddDiagnosticInfo(resourceManagerElement);
			if (!this.IsMonitoringEnabled)
			{
				return;
			}
			XElement xelement = new XElement("SmtpThrottlingController");
			resourceManagerElement.Add(xelement);
			this.smtpThrottlingController.AddDiagnosticInfo(xelement, showBasic);
			if (showBasic)
			{
				resourceManagerElement.Add(new object[]
				{
					new XElement("overallResourceUses", this.currentResourceUses),
					new XElement("privateBytesResourceUses", this.currentPrivateBytesUses),
					new XElement("versionBucketsResourceUses", this.currentVersionBucketUses),
					new XElement("versionBucketsPressureRaw", this.currentVersionBucketPressureRaw),
					new XElement("submissionQueueResourceUses", this.currentSubmissionQueueUses),
					new XElement("GarbageCollection", new object[]
					{
						new XElement("collectingGarbage", this.collectingGarbage),
						new XElement("gcCollectInterval", this.gcCollectInterval),
						new XElement("lastTimeGcCollectCalled", this.lastTimeGCCollectCalled),
						new XElement("lastGcCollectionCount", this.lastGCCollectionCount),
						new XElement("gcCollectLowInterval", ResourceManager.gcCollectLowInterval),
						new XElement("gcCollectHighInterval", ResourceManager.gcCollectHighInterval)
					})
				});
				XElement xelement2 = new XElement("ResourceMonitors", new XElement("count", this.resourceMonitors.Count));
				foreach (ResourceMonitor resourceMonitor in this.resourceMonitors)
				{
					xelement2.Add(resourceMonitor.GetDiagnosticInfo(showVerbose));
				}
				resourceManagerElement.Add(xelement2);
			}
			this.components.AddDiagnosticInfo(resourceManagerElement);
		}

		private static bool IsGCCollectionEffective(long lastMemoryChange)
		{
			long num = (long)(MemoryPrivateBytesMonitor.TotalPhysicalMemory / 100UL);
			return lastMemoryChange > num;
		}

		private void DoGarbageCollection(object obj)
		{
			try
			{
				ResourceMonitor resourceMonitor = this.resourceMonitors[3];
				ExTraceGlobals.ResourceManagerTracer.TraceDebug(0L, "Calling GC.Collect: PreviousResourceUses: {0}, CurrentResourceUses: {1}, CurrentPressureRaw: {2}, CurrentResourceUsesRaw: {3}", new object[]
				{
					resourceMonitor.PreviousResourceUses,
					resourceMonitor.ResourceUses,
					resourceMonitor.CurrentPressureRaw,
					resourceMonitor.CurrentResourceUsesRaw
				});
				long totalMemory = GC.GetTotalMemory(false);
				GC.Collect();
				this.lastTimeGCCollectCalled = DateTime.UtcNow;
				this.lastGCCollectionCount = GC.CollectionCount(2);
				long totalMemory2 = GC.GetTotalMemory(false);
				if (ResourceManager.IsGCCollectionEffective(totalMemory - totalMemory2))
				{
					this.gcCollectInterval = ResourceManager.gcCollectLowInterval;
				}
				else
				{
					this.gcCollectInterval = ResourceManager.gcCollectHighInterval;
				}
				ExTraceGlobals.ResourceManagerTracer.TraceDebug<ResourceUses, TimeSpan, long>(0L, "After GC.Collect: CurrentResourceUses: {0}, Next GC Interval: {1}, TotalFreedMemory: {2}", resourceMonitor.CurrentResourceUsesRaw, this.gcCollectInterval, totalMemory - totalMemory2);
			}
			finally
			{
				this.collectingGarbage = false;
			}
		}

		private void PostMonitorResource(ComponentsState requiredComponentsState)
		{
			ResourceUses resourceUses = this.currentResourceUses;
			try
			{
				this.updateLock.AcquireWriterLock(-1);
				this.currentPrivateBytesUses = this.resourceMonitors[3].ResourceUses;
				this.currentVersionBucketUses = this.resourceMonitors[2].ResourceUses;
				this.currentVersionBucketPressureRaw = this.resourceMonitors[2].CurrentPressureRaw;
				this.currentSubmissionQueueUses = this.resourceMonitors[5].ResourceUses;
				this.currentResourceUses = (from m in this.resourceMonitors
				where m != this.resourceMonitors[4]
				select m.ResourceUses).Max<ResourceUses>();
			}
			finally
			{
				this.updateLock.ReleaseWriterLock();
			}
			this.RefreshComponentsState(requiredComponentsState);
			if (DateTime.UtcNow - this.statusLastPublished > TimeSpan.FromMinutes(1.0) || this.currentResourceUses != resourceUses)
			{
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "RefreshComponentsState.Notification", null, this.currentResourceUses.ToString(), (this.currentResourceUses == ResourceUses.Normal) ? ResultSeverityLevel.Informational : ResultSeverityLevel.Error, false);
				if (this.currentResourceUses == resourceUses)
				{
					this.LogResourceUsePeriodic(this.aggregateResource, this.currentResourceUses, resourceUses);
				}
				this.statusLastPublished = DateTime.UtcNow;
			}
			if (this.currentResourceUses != resourceUses)
			{
				ResourceUses resourceUses2;
				bool flag = this.CheckToLogLowOnDiskSpace(out resourceUses2);
				this.eventLogger.LogResourcePressureChangedEvent(resourceUses, this.currentResourceUses, this.ToString());
				this.LogResourceUseChanged(this.aggregateResource, this.currentResourceUses, resourceUses);
				if (flag)
				{
					this.eventLogger.LogLowOnDiskSpaceEvent(resourceUses2, this.ToString());
				}
			}
			if (this.ShouldShrinkDownMemoryCaches)
			{
				if (this.resourceMonitors[3].ResourceUses > ResourceUses.Normal)
				{
					if (this.components.EnhancedDnsComponent != null)
					{
						this.components.EnhancedDnsComponent.FlushCache();
					}
					if (this.components.IsBridgehead)
					{
						Schema.FlushCache();
					}
					this.eventLogger.LogPrivateBytesHighEvent(this.ToString());
				}
				if (this.components.RemoteDeliveryComponent != null && this.resourceManagerConfig.DehydrateMessagesUnderMemoryPressure)
				{
					this.components.RemoteDeliveryComponent.CommitLazyAndDehydrateMessages();
				}
			}
		}

		private void LogResourceUseChanged(ResourceIdentifier resource, ResourceUses currentResourceUses, ResourceUses previousResourceUses)
		{
			ResourceUse resourceUse = new ResourceUse(resource, this.resourceUseToUseLevelMap[currentResourceUses], this.resourceUseToUseLevelMap[previousResourceUses]);
			this.resourceLog.LogResourceUseChange(resourceUse, null);
		}

		private void LogResourceUsePeriodic(ResourceIdentifier resource, ResourceUses currentResourceUses, ResourceUses previousResourceUses)
		{
			ResourceUse resourceUse = new ResourceUse(resource, this.resourceUseToUseLevelMap[currentResourceUses], this.resourceUseToUseLevelMap[previousResourceUses]);
			this.resourceLog.LogResourceUsePeriodic(resourceUse, null);
		}

		private bool CheckToLogLowOnDiskSpace(out ResourceUses maxResourceUsage)
		{
			maxResourceUsage = (from m in this.resourceMonitors.OfType<DiskSpaceMonitor>()
			where m.ResourceUses > m.PreviousResourceUses
			select m.ResourceUses).DefaultIfEmpty(ResourceUses.Normal).Max<ResourceUses>();
			return maxResourceUsage != ResourceUses.Normal;
		}

		private const int DatabaseMonitorIndex = 0;

		private const int PrivateBytesMonitorIndex = 3;

		private const int TotalBytesMonitorIndex = 4;

		private const int VersionBucketsMonitorIndex = 2;

		private const int DatabaseLoggingFolderMonitorIndex = 1;

		private const int SubmissionQueueMonitorIndex = 5;

		private static TimeSpan gcCollectLowInterval = TimeSpan.FromSeconds(5.0);

		private static TimeSpan gcCollectHighInterval = TimeSpan.FromSeconds(30.0);

		private ResourceManagerEventLogger eventLogger;

		private ResourceUses currentResourceUses;

		private ResourceUses currentPrivateBytesUses;

		private ResourceUses currentVersionBucketUses;

		private int currentVersionBucketPressureRaw;

		private ResourceUses currentSubmissionQueueUses;

		private List<ResourceMonitor> resourceMonitors = new List<ResourceMonitor>();

		private DateTime lastTimeResourceMonitored;

		private int lastGCCollectionCount = -1;

		private DateTime lastTimeGCCollectCalled = DateTime.MinValue;

		private TimeSpan gcCollectInterval = ResourceManager.gcCollectLowInterval;

		private volatile bool collectingGarbage;

		private FastReaderWriterLock updateLock = new FastReaderWriterLock();

		private readonly ResourceManagerComponentsAdapter components;

		private ThrottlingController smtpThrottlingController;

		private readonly ResourceManagerConfiguration resourceManagerConfig;

		private readonly ResourceMonitorFactory resourceMonitorFactory;

		private readonly ResourceManagerResources resourcesToMonitor;

		private readonly ResourceLog resourceLog;

		private readonly Dictionary<ResourceUses, UseLevel> resourceUseToUseLevelMap = new Dictionary<ResourceUses, UseLevel>
		{
			{
				ResourceUses.High,
				UseLevel.High
			},
			{
				ResourceUses.Medium,
				UseLevel.Medium
			},
			{
				ResourceUses.Normal,
				UseLevel.Low
			}
		};

		private readonly ResourceIdentifier aggregateResource = new ResourceIdentifier("Aggregate", "");

		private DateTime statusLastPublished;
	}
}
