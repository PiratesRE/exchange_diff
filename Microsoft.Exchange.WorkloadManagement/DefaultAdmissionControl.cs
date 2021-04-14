using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DefaultAdmissionControl : IResourceAdmissionControl
	{
		public DefaultAdmissionControl(ResourceKey resourceKey, RemoveResourceDelegate removeResourceDelegate, ResourceAvailabilityChangeDelegate resourceAvailabilityChanged, string owner)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("owner", owner);
			this.ResourceKey = resourceKey;
			this.resourceAvailabilityChanged = resourceAvailabilityChanged;
			this.removeResourceDelegate = removeResourceDelegate;
			this.lastRefreshUtc = null;
			this.id = string.Concat(new string[]
			{
				owner,
				"_",
				resourceKey.Id,
				"_",
				Guid.NewGuid().ToString("N")
			});
			this.classificationData = new ClassificationDictionary<AdmissionClassificationData>((WorkloadClassification classification) => new AdmissionClassificationData(classification, this.id));
			this.monitor = ResourceHealthMonitorManager.Singleton.Get(this.ResourceKey);
			ushort slotBlockedEventBucketCount = this.GetSlotBlockedEventBucketCount();
			this.slotBlockedEvent = new LogEventIfSlotBlocked(this.monitor, slotBlockedEventBucketCount);
		}

		public static Func<WorkloadClassification, bool> IsClassificationActiveDelegate { get; set; }

		public TimeSpan RefreshCycle
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.SystemWorkloadManager.RefreshCycle;
			}
		}

		public int MaxConcurrency
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.GetObject<IResourceSettings>(this.ResourceKey.MetricType, new object[0]).MaxConcurrency;
			}
		}

		public ResourceKey ResourceKey { get; private set; }

		public bool IsAcquired
		{
			get
			{
				lock (this.instanceLock)
				{
					foreach (AdmissionClassificationData admissionClassificationData in this.classificationData.Values)
					{
						if (admissionClassificationData.ConcurrencyUsed > 0)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		internal DateTime? LastRefreshUtc
		{
			get
			{
				return this.lastRefreshUtc;
			}
		}

		public bool TryAcquire(WorkloadClassification classification, out double delayFactor)
		{
			return this.TryAcquire(classification, DateTime.UtcNow, out delayFactor);
		}

		public void Release(WorkloadClassification classification)
		{
			this.Release(classification, DateTime.UtcNow);
		}

		internal bool TryAcquire(WorkloadClassification classification, DateTime utcNow, out double delayFactor)
		{
			int maxConcurrency = this.MaxConcurrency;
			this.VerifyOperational();
			this.Refresh(utcNow, classification);
			bool result;
			lock (this.instanceLock)
			{
				int num = 0;
				foreach (AdmissionClassificationData admissionClassificationData in this.classificationData.Values)
				{
					num += admissionClassificationData.ConcurrencyUsed;
					if (num >= maxConcurrency)
					{
						ExTraceGlobals.AdmissionControlTracer.TraceDebug<WorkloadClassification, int, ResourceKey>((long)this.GetHashCode(), "[DefaultAdmissionControl.TryAcquire] Unable to acquire slot for classification {0} because maximum concurrency {1} was reached for resource {2}.", classification, maxConcurrency, this.ResourceKey);
						delayFactor = 0.0;
						return false;
					}
				}
				AdmissionClassificationData admissionClassificationData2 = this.classificationData[classification];
				if (this.id.Equals(admissionClassificationData2.Id) && admissionClassificationData2.TryAquireSlot(out delayFactor))
				{
					this.RefreshAvailable(classification);
					result = true;
				}
				else
				{
					ExTraceGlobals.AdmissionControlTracer.TraceDebug((long)this.GetHashCode(), "[DefaultAdmissionControl.TryAcquire] Unable to acquire slot for classification {0} due to admission control limits.  Concurrency limit: {1}, Active slots: {2}, Resource: {3}", new object[]
					{
						classification,
						admissionClassificationData2.ConcurrencyLimits,
						admissionClassificationData2.ConcurrencyUsed,
						this.ResourceKey
					});
					delayFactor = 0.0;
					result = false;
				}
			}
			return result;
		}

		internal void Release(WorkloadClassification classification, DateTime utcNow)
		{
			this.VerifyOperational();
			this.Refresh(utcNow, classification);
			lock (this.instanceLock)
			{
				AdmissionClassificationData admissionClassificationData = this.classificationData[classification];
				if (this.id.Equals(admissionClassificationData.Id))
				{
					admissionClassificationData.ReleaseSlot();
					this.RefreshAvailable(classification);
				}
			}
		}

		internal int GetConcurrencyLimit(WorkloadClassification classification, out double delayFactor)
		{
			return this.GetConcurrencyLimit(classification, DateTime.UtcNow, out delayFactor);
		}

		internal int GetConcurrencyLimit(WorkloadClassification classification, DateTime utcNow, out double delayFactor)
		{
			this.VerifyOperational();
			this.Refresh(utcNow, classification);
			int concurrencyLimits;
			lock (this.instanceLock)
			{
				delayFactor = this.classificationData[classification].DelayFactor;
				concurrencyLimits = this.classificationData[classification].ConcurrencyLimits;
			}
			return concurrencyLimits;
		}

		internal int GetActiveConcurrency(WorkloadClassification classification)
		{
			this.VerifyOperational();
			int concurrencyUsed;
			lock (this.instanceLock)
			{
				concurrencyUsed = this.classificationData[classification].ConcurrencyUsed;
			}
			return concurrencyUsed;
		}

		internal bool GetClassificationAvailableAtLastRefresh(WorkloadClassification classification, DateTime utcNow)
		{
			this.VerifyOperational();
			this.Refresh(utcNow, classification);
			bool availableAtLastStatusChange;
			lock (this.instanceLock)
			{
				availableAtLastStatusChange = this.classificationData[classification].AvailableAtLastStatusChange;
			}
			return availableAtLastStatusChange;
		}

		internal void Test_Refresh(DateTime utcNow, WorkloadClassification classification)
		{
			this.Refresh(utcNow, classification);
		}

		private static bool ClassificationIsActive(WorkloadClassification classification)
		{
			if (DefaultAdmissionControl.IsClassificationActiveDelegate != null)
			{
				return DefaultAdmissionControl.IsClassificationActiveDelegate(classification);
			}
			return SystemWorkloadManager.Status != WorkloadExecutionStatus.NotInitialized && SystemWorkloadManager.IsClassificationActive(classification);
		}

		private static void SafeTouchMonitor(DefaultAdmissionControl control, Action action)
		{
			LocalizedException ex = null;
			try
			{
				action();
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				control.Remove();
				WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_AdmissionControlRefreshFailure, control.ResourceKey.ToString(), new object[]
				{
					control.ResourceKey,
					ex
				});
				throw new NonOperationalAdmissionControlException(control.ResourceKey, ex);
			}
		}

		private void Refresh(DateTime utcNow, WorkloadClassification classification)
		{
			TimeSpan refreshCycle = this.RefreshCycle;
			DefaultAdmissionControl.SafeTouchMonitor(this, delegate
			{
				this.monitor.GetResourceLoad(classification, false, null);
				lock (this.instanceLock)
				{
					TimeSpan t = (this.lastRefreshUtc != null) ? (this.monitor.LastUpdateUtc - this.lastRefreshUtc.Value) : TimeSpan.MaxValue;
					if (t <= TimeSpan.Zero)
					{
						this.slotBlockedEvent.Set(false);
					}
					else
					{
						this.slotBlockedEvent.Set(true);
					}
					if (this.lastRefreshUtc == null || utcNow - this.lastRefreshUtc.Value >= refreshCycle)
					{
						if (t <= TimeSpan.Zero)
						{
							ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, string, DateTime>((long)this.GetHashCode(), "[DefaultAdmissionControl.Refresh] Will not refresh slots because monitor '{0}' has not been updated since last refresh.  Last Refresh: {1}, MonitorUpdate: {2}", this.ResourceKey, (this.lastRefreshUtc != null) ? this.lastRefreshUtc.Value.ToString(CultureInfo.InvariantCulture) : "Never", this.monitor.LastUpdateUtc);
						}
						else
						{
							foreach (AdmissionClassificationData admissionClassificationData in this.classificationData.Values)
							{
								if (DefaultAdmissionControl.ClassificationIsActive(admissionClassificationData.Classification))
								{
									ResourceLoad resourceLoad = this.monitor.GetResourceLoad(admissionClassificationData.Classification, admissionClassificationData.ConcurrencyLimits > 1, null);
									int concurrencyLimits = admissionClassificationData.ConcurrencyLimits;
									admissionClassificationData.Refresh(this.ResourceKey, resourceLoad);
									if (concurrencyLimits > 0 && admissionClassificationData.ConcurrencyLimits == 0)
									{
										IResourceLoadNotification resourceLoadNotification = this.monitor as IResourceLoadNotification;
										if (resourceLoadNotification != null)
										{
											ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, WorkloadClassification>((long)this.GetHashCode(), "[DefaultAdmissionControl.Refresh] Registering for health notification - resource: {0}, Classification: {1}", this.ResourceKey, admissionClassificationData.Classification);
											resourceLoadNotification.SubscribeToHealthNotifications(admissionClassificationData.Classification, new HealthRecoveryNotification(this.HandleHealthRecovery));
										}
									}
									this.RefreshAvailable(admissionClassificationData.Classification);
									SystemWorkloadManagerLogEntry value = null;
									this.lastEntries.TryGetValue(admissionClassificationData.Classification, out value);
									SystemWorkloadManagerBlackBox.RecordAdmissionUpdate(ref value, this.ResourceKey, admissionClassificationData.Classification, resourceLoad, admissionClassificationData.ConcurrencyLimits, admissionClassificationData.DelayFactor > 0.0);
									this.lastEntries[admissionClassificationData.Classification] = value;
								}
							}
							this.lastRefreshUtc = new DateTime?(utcNow);
						}
					}
				}
			});
		}

		private void Remove()
		{
			lock (this.instanceLock)
			{
				this.operational = false;
				if (this.removeResourceDelegate != null)
				{
					this.removeResourceDelegate(this.ResourceKey);
				}
			}
		}

		private void HandleHealthRecovery(ResourceKey key, WorkloadClassification classification, Guid notificationCookie)
		{
			ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, WorkloadClassification>((long)this.GetHashCode(), "[DefaultAdmissionControl.HandleHealthRecovery] Resource '{0}' recovered for classification '{1}'.", key, classification);
			if (this.operational)
			{
				try
				{
					DefaultAdmissionControl.SafeTouchMonitor(this, delegate
					{
						this.RefreshHealthForClassification(classification);
					});
				}
				catch (NonOperationalAdmissionControlException)
				{
					ExTraceGlobals.AdmissionControlTracer.TraceDebug((long)this.GetHashCode(), "[DefaultAdmissionControl.HandleHealthRecovery] Caught NonOperationalAdmissionControlException though this is expected.  Ignoring.");
				}
			}
		}

		private void RefreshAvailable(WorkloadClassification classification)
		{
			bool flag2;
			bool availableAtLastStatusChange;
			lock (this.instanceLock)
			{
				AdmissionClassificationData admissionClassificationData = this.classificationData[classification];
				flag2 = admissionClassificationData.RefreshAvailable();
				availableAtLastStatusChange = admissionClassificationData.AvailableAtLastStatusChange;
			}
			if (flag2 && this.resourceAvailabilityChanged != null)
			{
				this.resourceAvailabilityChanged(this.ResourceKey, classification, availableAtLastStatusChange);
			}
		}

		private void RefreshHealthForClassification(WorkloadClassification classification)
		{
			bool flag2;
			bool availableAtLastStatusChange;
			lock (this.instanceLock)
			{
				AdmissionClassificationData admissionClassificationData = this.classificationData[classification];
				ResourceLoad resourceLoad = this.monitor.GetResourceLoad(admissionClassificationData.Classification, admissionClassificationData.ConcurrencyLimits > 1, null);
				admissionClassificationData.Refresh(this.ResourceKey, resourceLoad);
				flag2 = admissionClassificationData.RefreshAvailable();
				availableAtLastStatusChange = admissionClassificationData.AvailableAtLastStatusChange;
			}
			if (flag2 && this.resourceAvailabilityChanged != null)
			{
				this.resourceAvailabilityChanged(this.ResourceKey, classification, availableAtLastStatusChange);
			}
		}

		private void VerifyOperational()
		{
			if (!this.operational)
			{
				throw new NonOperationalAdmissionControlException(this.ResourceKey);
			}
		}

		private ushort GetSlotBlockedEventBucketCount()
		{
			int val = (int)Math.Ceiling(this.RefreshCycle.TotalMinutes);
			return (ushort)Math.Max(val, 5);
		}

		private const int MinimumBucketsForSlotBlockedEvent = 5;

		private readonly string id = string.Empty;

		private IResourceLoadMonitor monitor;

		private object instanceLock = new object();

		private ClassificationDictionary<AdmissionClassificationData> classificationData;

		private DateTime? lastRefreshUtc;

		private ResourceAvailabilityChangeDelegate resourceAvailabilityChanged;

		private RemoveResourceDelegate removeResourceDelegate;

		private LogEventIfSlotBlocked slotBlockedEvent;

		private bool operational = true;

		private ClassificationDictionary<SystemWorkloadManagerLogEntry> lastEntries = new ClassificationDictionary<SystemWorkloadManagerLogEntry>();
	}
}
