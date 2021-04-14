using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal abstract class CacheableResourceHealthMonitor
	{
		public static TimeSpan NotificationCheckFrequency { get; set; } = CacheableResourceHealthMonitor.DefaultNotificationCheckFrequency;

		static CacheableResourceHealthMonitor()
		{
			IntAppSettingsEntry intAppSettingsEntry = new IntAppSettingsEntry("ResourceMonitor.NumberOfAdjustmentSteps", 5, ExTraceGlobals.ResourceHealthManagerTracer);
			CacheableResourceHealthMonitor.numberOfAdjustmentSteps = intAppSettingsEntry.Value;
		}

		protected CacheableResourceHealthMonitor(ResourceKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.resourceKey = key;
			this.notifications = new List<CacheableResourceHealthMonitor.NotificationWrapper>();
			this.notificationCount = 0;
			this.LastAccessUtc = TimeProvider.UtcNow;
			this.CreationStack = new StackTrace().ToString();
		}

		public virtual ResourceHealthMonitorWrapper CreateWrapper()
		{
			return new ResourceHealthMonitorWrapper(this);
		}

		public virtual bool Expired { get; private set; }

		public ResourceKey Key
		{
			get
			{
				return this.resourceKey;
			}
		}

		public virtual DateTime LastAccessUtc { get; protected set; }

		internal string CreationStack { get; private set; }

		internal void Expire()
		{
			this.Expired = true;
			this.HandleExpired();
		}

		protected virtual void HandleExpired()
		{
		}

		public virtual bool ShouldRemoveResourceFromCache()
		{
			return this.notificationCount == 0;
		}

		public Guid SubscribeToHealthNotifications(WorkloadClassification classification, HealthRecoveryNotification delegateToFire)
		{
			if (delegateToFire == null)
			{
				throw new ArgumentNullException("delegateToFire");
			}
			if (classification == WorkloadClassification.Unknown)
			{
				throw new ArgumentException("You cannot use Unknown to register for health change notifications.", "classification");
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey, WorkloadClassification>((long)this.GetHashCode(), "[CacheableResourceHealthMonitor.SubscribeToHealthNotifications] Registration was made for resource '{0}', classification: {1}.", this.Key, classification);
			Guid key;
			lock (this.instanceLock)
			{
				CacheableResourceHealthMonitor.NotificationWrapper notificationWrapper = new CacheableResourceHealthMonitor.NotificationWrapper(classification, delegateToFire);
				this.notifications.Add(notificationWrapper);
				this.notificationCount++;
				key = notificationWrapper.Key;
			}
			return key;
		}

		public bool UnsubscribeFromHealthNotifications(Guid registrationKey)
		{
			if (registrationKey == Guid.Empty)
			{
				throw new ArgumentException("Guid.Empty is never an acceptable registration key", "registrationKey");
			}
			bool flag = false;
			lock (this.instanceLock)
			{
				if (this.notificationCount > 0)
				{
					for (int i = this.notifications.Count - 1; i >= 0; i--)
					{
						CacheableResourceHealthMonitor.NotificationWrapper notificationWrapper = this.notifications[i];
						if (notificationWrapper.Key == registrationKey)
						{
							this.notifications.RemoveAt(i);
							this.notificationCount--;
							flag = true;
							break;
						}
					}
				}
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey, Guid, bool>((long)this.GetHashCode(), "[CacheableResourceHealthMonitor.UnsubscribeFromHealthNotifications] Unregistered for Resource Key: '{0}', Guid: '{1}', Found? {2}", this.Key, registrationKey, flag);
			return flag;
		}

		public int HealthSubscriptionCount
		{
			get
			{
				return this.notificationCount;
			}
		}

		internal int FireNotifications()
		{
			int num = 0;
			List<CacheableResourceHealthMonitor.NotificationWrapper> list = null;
			if (CacheableResourceHealthMonitor.NotificationCheckFrequency == TimeSpan.Zero || TimeProvider.UtcNow - this.lastNotificationCheckUtc >= CacheableResourceHealthMonitor.NotificationCheckFrequency)
			{
				lock (this.instanceLock)
				{
					if (CacheableResourceHealthMonitor.NotificationCheckFrequency == TimeSpan.Zero || TimeProvider.UtcNow - this.lastNotificationCheckUtc >= CacheableResourceHealthMonitor.NotificationCheckFrequency)
					{
						for (int i = this.notifications.Count - 1; i >= 0; i--)
						{
							CacheableResourceHealthMonitor.NotificationWrapper notificationWrapper = this.notifications[i];
							if (this.GetResourceLoad(notificationWrapper.Classification, false, null).State == ResourceLoadState.Underloaded)
							{
								if (list == null)
								{
									list = new List<CacheableResourceHealthMonitor.NotificationWrapper>();
								}
								list.Add(notificationWrapper);
								this.notificationCount--;
								num++;
								this.notifications.RemoveAt(i);
							}
						}
					}
					this.lastNotificationCheckUtc = TimeProvider.UtcNow;
				}
			}
			if (list != null)
			{
				this.FireNotificationsAsync(list);
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int, ResourceKey>((long)this.GetHashCode(), "[CacheableResourceHealthMonitor.FireNotifications] Firing off {0} notifications for resource '{1}'", num, this.Key);
			}
			return num;
		}

		private void FireNotificationsAsync(List<CacheableResourceHealthMonitor.NotificationWrapper> notifications)
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				foreach (CacheableResourceHealthMonitor.NotificationWrapper notificationWrapper in notifications)
				{
					notificationWrapper.Notification(this.Key, notificationWrapper.Classification, notificationWrapper.Key);
				}
			});
		}

		public ResourceMetricType MetricType
		{
			get
			{
				return this.Key.MetricType;
			}
		}

		public virtual DateTime LastUpdateUtc
		{
			get
			{
				if (!this.Settings.Enabled)
				{
					return DateTime.UtcNow;
				}
				return this.lastUpdateUtc;
			}
			protected internal set
			{
				this.lastUpdateUtc = value;
			}
		}

		public int MetricValue
		{
			get
			{
				this.LastAccessUtc = TimeProvider.UtcNow;
				return this.InternalMetricValue;
			}
		}

		protected abstract int InternalMetricValue { get; }

		public int GetMetricValue(object optionalData)
		{
			this.LastAccessUtc = TimeProvider.UtcNow;
			return this.InternalGetMetricValue(optionalData);
		}

		protected virtual int InternalGetMetricValue(object optionalData)
		{
			return this.InternalMetricValue;
		}

		protected IResourceSettings Settings
		{
			get
			{
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
				return snapshot.WorkloadManagement.GetObject<IResourceSettings>(this.MetricType, new object[0]);
			}
		}

		public virtual ResourceLoad GetResourceLoad(WorkloadType type, bool raw = false, object optionalData = null)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			IWorkloadSettings @object = snapshot.WorkloadManagement.GetObject<IWorkloadSettings>(type, new object[0]);
			return this.GetResourceLoad(@object.Classification, raw, optionalData);
		}

		public virtual ResourceLoad GetResourceLoad(WorkloadClassification classification, bool raw = false, object optionalData = null)
		{
			if (!this.Settings.Enabled)
			{
				return ResourceLoad.Zero;
			}
			int metricValue = this.GetMetricValue(optionalData);
			if (metricValue < 0)
			{
				return ResourceLoad.Unknown;
			}
			ResourceMetricPolicy resourceMetricPolicy = new ResourceMetricPolicy(this.MetricType, classification, this.Settings);
			ResourceLoad resourceLoad = resourceMetricPolicy.InterpretMetricValue(metricValue);
			if (!raw)
			{
				CacheableResourceHealthMonitor.LoadInfo loadInfo = this.loadInfo[classification];
				if (this.UpdateIsNeeded(loadInfo, resourceLoad))
				{
					try
					{
						if (Monitor.TryEnter(this.instanceLock) && this.UpdateIsNeeded(loadInfo, resourceLoad))
						{
							switch (resourceLoad.State)
							{
							case ResourceLoadState.Unknown:
								loadInfo.Load = resourceLoad;
								break;
							case ResourceLoadState.Underloaded:
							case ResourceLoadState.Full:
								if (loadInfo.Load < ResourceLoad.Full)
								{
									loadInfo.Load = resourceLoad;
								}
								else if (this.LastUpdateUtc > loadInfo.UpdateUtc)
								{
									if (loadInfo.Load == ResourceLoad.Full)
									{
										loadInfo.Load = resourceLoad;
									}
									else if (loadInfo.Load == ResourceLoad.Critical)
									{
										loadInfo.Load = new ResourceLoad(resourceMetricPolicy.MaxOverloaded.LoadRatio, new int?(metricValue), null);
									}
									else if (resourceLoad.State == ResourceLoadState.Underloaded)
									{
										double num = (resourceMetricPolicy.MaxOverloaded - ResourceLoad.Full) / (double)CacheableResourceHealthMonitor.numberOfAdjustmentSteps;
										if (loadInfo.Load - ResourceLoad.Full > num)
										{
											loadInfo.Load -= num;
										}
										else
										{
											loadInfo.Load = new ResourceLoad(ResourceLoad.Full.LoadRatio, new int?(metricValue), null);
										}
									}
								}
								break;
							case ResourceLoadState.Overloaded:
								if (loadInfo.Load < resourceLoad)
								{
									loadInfo.Load = resourceLoad;
								}
								else if (this.LastUpdateUtc > loadInfo.UpdateUtc)
								{
									double delta = (resourceMetricPolicy.MaxOverloaded - ResourceLoad.Full) / (double)CacheableResourceHealthMonitor.numberOfAdjustmentSteps;
									if (loadInfo.Load + delta <= resourceMetricPolicy.MaxOverloaded)
									{
										loadInfo.Load += delta;
									}
									else
									{
										loadInfo.Load = new ResourceLoad(ResourceLoad.Critical.LoadRatio, new int?(metricValue), null);
									}
								}
								break;
							case ResourceLoadState.Critical:
								loadInfo.Load = resourceLoad;
								break;
							}
						}
					}
					finally
					{
						if (Monitor.IsEntered(this.instanceLock))
						{
							Monitor.Exit(this.instanceLock);
						}
					}
				}
				resourceLoad = loadInfo.Load;
			}
			object resourceLoadlInfo = this.GetResourceLoadlInfo(resourceLoad);
			if (resourceLoadlInfo != null)
			{
				resourceLoad = new ResourceLoad(resourceLoad.LoadRatio, resourceLoad.Metric, resourceLoadlInfo);
			}
			ResourceLoadPerfCounterWrapper.Get(this.resourceKey, classification).Update(metricValue, resourceLoad);
			lock (this.lastEntries)
			{
				SystemWorkloadManagerLogEntry value = null;
				this.lastEntries.TryGetValue(classification, out value);
				SystemWorkloadManagerBlackBox.RecordMonitorUpdate(ref value, this.resourceKey, classification, resourceLoad);
				this.lastEntries[classification] = value;
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceMetricType, ResourceLoad>((long)this.GetHashCode(), "[ResourceLoadMonitor.GetResourceLoad] MetricType={0}, Current Load={1}.", this.MetricType, resourceLoad);
			return resourceLoad;
		}

		protected virtual object GetResourceLoadlInfo(ResourceLoad load)
		{
			return null;
		}

		private static Dictionary<WorkloadClassification, CacheableResourceHealthMonitor.LoadInfo> CreateLoadInfoDictionary()
		{
			Dictionary<WorkloadClassification, CacheableResourceHealthMonitor.LoadInfo> dictionary = new Dictionary<WorkloadClassification, CacheableResourceHealthMonitor.LoadInfo>();
			foreach (object obj in Enum.GetValues(typeof(WorkloadClassification)))
			{
				WorkloadClassification workloadClassification = (WorkloadClassification)obj;
				if (workloadClassification != WorkloadClassification.Unknown)
				{
					dictionary.Add(workloadClassification, new CacheableResourceHealthMonitor.LoadInfo());
				}
			}
			return dictionary;
		}

		private bool UpdateIsNeeded(CacheableResourceHealthMonitor.LoadInfo loadInfo, ResourceLoad newLoad)
		{
			return (loadInfo.Load == ResourceLoad.Unknown && newLoad != ResourceLoad.Unknown) || this.LastUpdateUtc > loadInfo.UpdateUtc || newLoad == ResourceLoad.Critical || newLoad > loadInfo.Load;
		}

		private const int DefaultNumberOfAdjustmentSteps = 5;

		private const string ConfigNumberOfAdjustmentSteps = "ResourceMonitor.NumberOfAdjustmentSteps";

		protected const int UnknownMetricValue = -1;

		private static readonly TimeSpan DefaultNotificationCheckFrequency = TimeSpan.FromSeconds(10.0);

		private static int numberOfAdjustmentSteps = 5;

		private ResourceKey resourceKey;

		private object instanceLock = new object();

		private List<CacheableResourceHealthMonitor.NotificationWrapper> notifications;

		private int notificationCount;

		private DateTime lastNotificationCheckUtc = TimeProvider.UtcNow;

		private DateTime lastUpdateUtc = DateTime.MinValue;

		private Dictionary<WorkloadClassification, SystemWorkloadManagerLogEntry> lastEntries = new Dictionary<WorkloadClassification, SystemWorkloadManagerLogEntry>();

		private Dictionary<WorkloadClassification, CacheableResourceHealthMonitor.LoadInfo> loadInfo = CacheableResourceHealthMonitor.CreateLoadInfoDictionary();

		private class NotificationWrapper
		{
			public NotificationWrapper(WorkloadClassification classification, HealthRecoveryNotification delegateToFire)
			{
				this.Classification = classification;
				this.Notification = delegateToFire;
				this.Key = Guid.NewGuid();
			}

			public HealthRecoveryNotification Notification { get; private set; }

			public Guid Key { get; private set; }

			public WorkloadClassification Classification { get; private set; }
		}

		private class LoadInfo
		{
			public LoadInfo()
			{
				this.Load = ResourceLoad.Unknown;
			}

			public DateTime UpdateUtc { get; private set; }

			public ResourceLoad Load
			{
				get
				{
					return this.load;
				}
				set
				{
					this.load = value;
					this.UpdateUtc = TimeProvider.UtcNow;
				}
			}

			private ResourceLoad load;
		}
	}
}
