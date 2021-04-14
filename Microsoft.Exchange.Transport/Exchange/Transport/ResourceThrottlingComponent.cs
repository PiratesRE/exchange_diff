using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.ResourceMonitoring;
using Microsoft.Exchange.Transport.ResourceThrottling;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ResourceThrottlingComponent : ITransportComponent, IDiagnosable
	{
		public ResourceThrottlingComponent(ResourceMeteringConfig resourceMeteringConfig, ResourceThrottlingConfig resourceThrottlingConfig, IComponentsWrapper componentsWrapper, MessagingDatabaseComponent messagingDatabaseComponent, CategorizerComponent categorizerComponent, ITransportConfiguration configComponent, ResourceManagerResources resourcesToMonitor, ResourceObservingComponents observingComponents)
		{
			ArgumentValidator.ThrowIfNull("resourceMeteringConfig", resourceMeteringConfig);
			ArgumentValidator.ThrowIfNull("resourceThrottlingConfig", resourceThrottlingConfig);
			ArgumentValidator.ThrowIfNull("componentsWrapper", componentsWrapper);
			ArgumentValidator.ThrowIfNull("messagingDatabaseComponent", messagingDatabaseComponent);
			ArgumentValidator.ThrowIfNull("configComponent", configComponent);
			this.resourceMeteringConfig = resourceMeteringConfig;
			this.resourceThrottlingConfig = resourceThrottlingConfig;
			this.componentsWrapper = componentsWrapper;
			this.messagingDatabaseComponent = messagingDatabaseComponent;
			this.categorizerComponent = categorizerComponent;
			this.configComponent = configComponent;
			this.resourcesToMonitor = resourcesToMonitor;
			this.observingComponents = observingComponents;
			ResourceIdentifier resourceIdentifier = new ResourceIdentifier("QueueLength", "SubmissionQueue");
			this.resourceMeterCreators.Add("PrivateBytes", new Func<ResourceIdentifier, PressureTransitions, IResourceMeter>(this.CreatePrivateBytesResourceMeter));
			this.resourceMeterCreators.Add("DatabaseUsedSpace", new Func<ResourceIdentifier, PressureTransitions, IResourceMeter>(this.CreateDatabaseUsedSpaceResourceMeter));
			this.resourceMeterCreators.Add(resourceIdentifier.ToString(), new Func<ResourceIdentifier, PressureTransitions, IResourceMeter>(this.CreateSubmissionQueueLengthResourceMeter));
			this.resourceMeterCreators.Add("UsedVersionBuckets", new Func<ResourceIdentifier, PressureTransitions, IResourceMeter>(this.CreateUsedVersionBucketsResourceMeter));
			this.resourceMeterCreators.Add("SystemMemory", new Func<ResourceIdentifier, PressureTransitions, IResourceMeter>(this.CreateSystemMemoryResourceMeter));
			this.resourceMeterCreators.Add("UsedDiskSpace", new Func<ResourceIdentifier, PressureTransitions, IResourceMeter>(this.CreateUsedDiskSpaceMeter));
		}

		public void PublishStatus()
		{
			if (this.ShouldPublishStatus())
			{
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "RefreshComponentsState.Notification", null, this.resourceTracker.AggregateResourceUse.CurrentUseLevel.ToString(), (this.resourceTracker.AggregateResourceUse.CurrentUseLevel == UseLevel.Low) ? ResultSeverityLevel.Informational : ResultSeverityLevel.Error, false);
				string backPressureDescription = this.GetBackPressureDescription();
				this.eventLogger.LogResourcePressureChangedEvent(this.resourceTracker.AggregateResourceUse, backPressureDescription);
				ResourceUse resourceUse2 = this.resourceTracker.ResourceUses.SingleOrDefault((ResourceUse resourceUse) => resourceUse.Resource == this.privateBytesResource);
				if (resourceUse2 != null && resourceUse2.CurrentUseLevel != UseLevel.Low)
				{
					this.eventLogger.LogPrivateBytesHighEvent(backPressureDescription);
				}
				IEnumerable<ResourceUse> source = from resourceUse in this.resourceTracker.ResourceUses
				where resourceUse.Resource.Name == "UsedDiskSpace"
				select resourceUse;
				if (source.Any<ResourceUse>())
				{
					UseLevel useLevel = source.Max((ResourceUse resourceUse) => resourceUse.CurrentUseLevel);
					if (useLevel != UseLevel.Low)
					{
						this.eventLogger.LogLowOnDiskSpaceEvent(useLevel, backPressureDescription);
					}
				}
				this.statusLastPublishTime = DateTime.UtcNow;
			}
		}

		private bool ShouldPublishStatus()
		{
			bool result = false;
			if (DateTime.UtcNow - this.statusLastPublishTime > this.resourceMeteringConfig.StatusPublishInterval)
			{
				result = true;
			}
			else if (this.statusLastPublishTime < this.resourceTracker.LastUpdateTime && this.resourceTracker.AggregateResourceUse.CurrentUseLevel != this.resourceTracker.AggregateResourceUse.PreviousUseLevel)
			{
				result = true;
			}
			return result;
		}

		public void ObserveExceptions()
		{
			if (this.resourceTrackingTask != null && this.resourceTrackingTask.IsCompleted && this.resourceTrackingTask.Exception != null)
			{
				throw this.resourceTrackingTask.Exception;
			}
		}

		void ITransportComponent.Load()
		{
			if (!this.resourceMeteringConfig.IsResourceTrackingEnabled)
			{
				return;
			}
			this.cancellationTokenSource = new CancellationTokenSource();
			ResourceLog resourceLog = this.CreateResourceLog();
			this.GetResourcesAndPressureTransitions();
			List<IResourceMeter> list = new List<IResourceMeter>();
			foreach (ResourceIdentifier resource in this.meteredResources)
			{
				list.Add(this.CreateResourceMeter(resource));
			}
			this.resourceTracker = new ResourceTracker(list, this.resourceMeteringConfig.ResourceMeteringInterval, this.resourceMeteringConfig.ResourceMeterTimeout, resourceLog, this.resourceMeteringConfig.ResourceLoggingInterval, new Func<IEnumerable<ResourceUse>, UseLevel>(this.GetAggregateUseLevel), this.resourceMeteringConfig.MaxTransientExceptionsAllowed);
			if (this.resourceThrottlingConfig.IsResourceThrottlingEnabled)
			{
				IEnumerable<IResourceLevelObserver> resourceLevelObservers = this.GetResourceLevelObservers();
				this.resourceLevelMediator = new ResourceLevelMediator(this.resourceTracker, resourceLevelObservers, this.resourceThrottlingConfig.ResourceObserverTimeout, this.resourceThrottlingConfig.MaxTransientExceptionsAllowed);
			}
			int maxSamples = 1;
			if (this.resourceMeteringConfig.SustainedDuration > this.resourceMeteringConfig.ResourceMeteringInterval)
			{
				maxSamples = (int)(this.resourceMeteringConfig.SustainedDuration.TotalSeconds / this.resourceMeteringConfig.ResourceMeteringInterval.TotalSeconds);
			}
			this.sustainedBackPressureStabilizer = new ResourceSampleStabilizer(maxSamples, new ResourceSample(UseLevel.Low, 0L));
			this.perfCountersInstance = ResourceThrottlingPerfCounters.GetInstance(Process.GetCurrentProcess().ProcessName);
			this.resourceTrackingTask = this.resourceTracker.StartResourceTrackingAsync(this.cancellationTokenSource.Token);
			this.statusTimer = new GuardedTimer(new TimerCallback(this.StatusUpdate), null, this.resourceMeteringConfig.ResourceMeteringInterval);
		}

		private UseLevel GetAggregateUseLevel(IEnumerable<ResourceUse> resourceUses)
		{
			UseLevel result = UseLevel.Low;
			if (resourceUses != null && resourceUses.Any<ResourceUse>())
			{
				ResourceIdentifier systemMemory = new ResourceIdentifier("SystemMemory", "");
				IEnumerable<ResourceUse> source = from use in resourceUses
				where use.Resource != systemMemory
				select use;
				if (source.Any<ResourceUse>())
				{
					result = source.Max((ResourceUse use) => use.CurrentUseLevel);
				}
			}
			return result;
		}

		private void StatusUpdate(object state)
		{
			if (this.resourceMeteringConfig.IsResourceTrackingEnabled)
			{
				this.ObserveExceptions();
				this.CollectDiagnostics();
				this.PublishStatus();
				this.PublishDiagnostics();
			}
		}

		private void CollectDiagnostics()
		{
			this.sustainedBackPressureStabilizer.AddSample(new ResourceSample(this.resourceTracker.AggregateResourceUse.CurrentUseLevel, 0L));
			if (this.sustainedBackPressureStabilizer.GetUseLevelPercentage(UseLevel.Low) < 15)
			{
				if (this.backPressureStartTime == DateTime.MaxValue)
				{
					this.backPressureStartTime = DateTime.UtcNow - this.resourceMeteringConfig.SustainedDuration;
				}
			}
			else
			{
				this.backPressureStartTime = DateTime.MaxValue;
			}
			ResourceTrackerDiagnosticsData diagnosticsData = this.resourceTracker.GetDiagnosticsData();
			this.maxResourceMeterCallDuration = TimeSpan.Zero;
			foreach (IResourceMeter resourceMeter in this.resourceTracker.ResourceMeters)
			{
				TimeSpan resourceMeterCallDuration = diagnosticsData.GetResourceMeterCallDuration(resourceMeter.Resource);
				if (resourceMeterCallDuration > this.maxResourceMeterCallDuration)
				{
					this.maxResourceMeterCallDuration = resourceMeterCallDuration;
				}
			}
			ResourceLevelMediatorDiagnosticsData diagnosticsData2 = this.resourceLevelMediator.GetDiagnosticsData();
			this.maxResourceObserverCallDuration = TimeSpan.Zero;
			foreach (IResourceLevelObserver resourceLevelObserver in this.resourceLevelMediator.ResourceLevelObservers)
			{
				TimeSpan resourceObserverCallDuration = diagnosticsData2.GetResourceObserverCallDuration(resourceLevelObserver.Name);
				if (resourceObserverCallDuration > this.maxResourceObserverCallDuration)
				{
					this.maxResourceObserverCallDuration = resourceObserverCallDuration;
				}
			}
		}

		private void PublishDiagnostics()
		{
			if (this.backPressureStartTime >= DateTime.UtcNow)
			{
				this.perfCountersInstance.BackPressureTime.RawValue = 0L;
			}
			else
			{
				this.perfCountersInstance.BackPressureTime.RawValue = (long)(DateTime.UtcNow - this.backPressureStartTime).TotalSeconds;
			}
			this.perfCountersInstance.ResourceMeterLongestCallDuration.RawValue = (long)this.maxResourceMeterCallDuration.TotalMilliseconds;
			this.perfCountersInstance.ResourceObserverLongestCallDuration.RawValue = (long)this.maxResourceObserverCallDuration.TotalMilliseconds;
		}

		public bool IsResourceTrackingEnabled
		{
			get
			{
				return this.resourceMeteringConfig.IsResourceTrackingEnabled;
			}
		}

		private void GetResourcesAndPressureTransitions()
		{
			this.meteredResources = new List<ResourceIdentifier>();
			if ((this.resourcesToMonitor & ResourceManagerResources.PrivateBytes) > ResourceManagerResources.None)
			{
				this.meteredResources.Add(this.privateBytesResource);
				this.localizedResourceNames.Add(this.privateBytesResource, Strings.PrivateBytesResource);
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.SubmissionQueue) > ResourceManagerResources.None)
			{
				ResourceIdentifier resourceIdentifier = new ResourceIdentifier("QueueLength", "SubmissionQueue");
				this.meteredResources.Add(resourceIdentifier);
				this.localizedResourceNames.Add(resourceIdentifier, Strings.QueueLength(Strings.Submission));
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.TotalBytes) > ResourceManagerResources.None)
			{
				ResourceIdentifier resourceIdentifier2 = new ResourceIdentifier("SystemMemory", "");
				this.meteredResources.Add(resourceIdentifier2);
				this.localizedResourceNames.Add(resourceIdentifier2, Strings.SystemMemory);
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.VersionBuckets) > ResourceManagerResources.None)
			{
				string databasePath = this.messagingDatabaseComponent.Database.DataSource.DatabasePath;
				ResourceIdentifier resourceIdentifier3 = new ResourceIdentifier("UsedVersionBuckets", databasePath);
				this.meteredResources.Add(resourceIdentifier3);
				this.localizedResourceNames.Add(resourceIdentifier3, Strings.VersionBuckets(databasePath));
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.MailDatabase) > ResourceManagerResources.None)
			{
				string databasePath2 = this.messagingDatabaseComponent.Database.DataSource.DatabasePath;
				string directoryName = Path.GetDirectoryName(databasePath2);
				ResourceIdentifier resourceIdentifier4 = new ResourceIdentifier("DatabaseUsedSpace", directoryName);
				this.meteredResources.Add(resourceIdentifier4);
				this.localizedResourceNames.Add(resourceIdentifier4, Strings.DatabaseResource(databasePath2));
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.MailDatabaseLoggingFolder) > ResourceManagerResources.None)
			{
				string logFilePath = this.messagingDatabaseComponent.Database.DataSource.LogFilePath;
				string directoryName2 = Path.GetDirectoryName(logFilePath);
				ResourceIdentifier resourceIdentifier5 = new ResourceIdentifier("UsedDiskSpace", directoryName2);
				if (!this.meteredResources.Contains(resourceIdentifier5))
				{
					this.meteredResources.Add(resourceIdentifier5);
					this.localizedResourceNames.Add(resourceIdentifier5, Strings.UsedDiskSpaceResource(directoryName2));
				}
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.TempDrive) > ResourceManagerResources.None)
			{
				string directoryName3 = Path.GetDirectoryName(Components.TransportAppConfig.WorkerProcess.TemporaryStoragePath);
				ResourceIdentifier resourceIdentifier6 = new ResourceIdentifier("UsedDiskSpace", directoryName3);
				if (!this.meteredResources.Contains(resourceIdentifier6))
				{
					this.meteredResources.Add(resourceIdentifier6);
					this.localizedResourceNames.Add(resourceIdentifier6, Strings.UsedDiskSpaceResource(directoryName3));
				}
			}
			this.pressureTransitionsForResources = this.resourceMeteringConfig.GetPressureTransitionsForResources(this.meteredResources);
			this.AdjustDiskSpaceResources();
			ResourceIdentifier key = new ResourceIdentifier("Aggregate", "");
			this.localizedResourceNames.Add(key, Strings.AggregateResource);
		}

		private void AdjustDiskSpaceResources()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			long num = 0L;
			if ((this.resourcesToMonitor & ResourceManagerResources.MailDatabase) > ResourceManagerResources.None)
			{
				text2 = Path.GetDirectoryName(this.messagingDatabaseComponent.Database.DataSource.DatabasePath);
				ResourceIdentifier key = new ResourceIdentifier("DatabaseUsedSpace", text2);
				if (this.pressureTransitionsForResources.ContainsKey(key) && this.pressureTransitionsForResources[key].MediumToHigh == 100L)
				{
					this.pressureTransitionsForResources[key] = this.EnsureMinDiskSpaceRequired(text2, this.pressureTransitionsForResources[key], (long)ByteQuantifiedSize.FromMB(500UL).ToBytes());
				}
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.MailDatabaseLoggingFolder) > ResourceManagerResources.None)
			{
				text = Path.GetDirectoryName(this.messagingDatabaseComponent.Database.DataSource.LogFilePath);
				ResourceIdentifier key2 = new ResourceIdentifier("UsedDiskSpace", text);
				if (this.pressureTransitionsForResources.ContainsKey(key2))
				{
					num = Math.Min((long)(Components.TransportAppConfig.JetDatabase.CheckpointDepthMax.ToBytes() * 3UL), (long)ByteQuantifiedSize.FromGB(5UL).ToBytes());
					this.pressureTransitionsForResources[key2] = this.EnsureMinDiskSpaceRequired(text, this.pressureTransitionsForResources[key2], num);
				}
			}
			if ((this.resourcesToMonitor & ResourceManagerResources.TempDrive) > ResourceManagerResources.None)
			{
				string directoryName = Path.GetDirectoryName(Components.TransportAppConfig.WorkerProcess.TemporaryStoragePath);
				ResourceIdentifier key3 = new ResourceIdentifier("UsedDiskSpace", directoryName);
				if (this.pressureTransitionsForResources.ContainsKey(key3))
				{
					long num2 = (long)Components.TransportAppConfig.ResourceManager.TempDiskSpaceRequired.ToBytes();
					if (string.Compare(text, directoryName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num2 = Math.Max(num2, num);
					}
					this.pressureTransitionsForResources[key3] = this.EnsureMinDiskSpaceRequired(directoryName, this.pressureTransitionsForResources[key3], num2);
				}
			}
		}

		private PressureTransitions EnsureMinDiskSpaceRequired(string path, PressureTransitions pressureTransitions, long minSpaceRequired)
		{
			INativeMethodsWrapper nativeMethodsWrapper = NativeMethodsWrapperFactory.CreateNativeMethodsWrapper();
			ulong num;
			ulong num2;
			ulong num3;
			if (nativeMethodsWrapper.GetDiskFreeSpaceEx(path, out num, out num2, out num3))
			{
				long val = (long)((num2 - (ulong)minSpaceRequired) * 100UL / num2);
				long num4 = Math.Min(pressureTransitions.MediumToHigh, val);
				long num5 = Math.Min(pressureTransitions.HighToMedium, num4 - 2L);
				long num6 = Math.Min(pressureTransitions.LowToMedium, num5 - 1L);
				long mediumToLow = Math.Min(pressureTransitions.MediumToLow, num6 - 2L);
				return new PressureTransitions(num4, num5, num6, mediumToLow);
			}
			return pressureTransitions;
		}

		private ResourceLog CreateResourceLog()
		{
			Server transportServer = this.configComponent.LocalServer.TransportServer;
			bool enabled = transportServer.ResourceLogEnabled;
			string logDirectory = string.Empty;
			if (transportServer.ResourceLogPath == null || string.IsNullOrEmpty(transportServer.ResourceLogPath.PathName))
			{
				enabled = false;
			}
			else
			{
				logDirectory = transportServer.ResourceLogPath.PathName;
			}
			return new ResourceLog(enabled, "ResourceThrottling", logDirectory, transportServer.ResourceLogMaxAge, this.resourceMeteringConfig.ResourceLogFlushInterval, this.resourceMeteringConfig.ResourceLogBackgroundWriteInterval, (long)(transportServer.ResourceLogMaxDirectorySize.IsUnlimited ? 0UL : transportServer.ResourceLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.ResourceLogMaxFileSize.IsUnlimited ? 0UL : transportServer.ResourceLogMaxDirectorySize.Value.ToBytes()), this.resourceMeteringConfig.ResourceLogBufferSize);
		}

		private IEnumerable<IResourceLevelObserver> GetResourceLevelObservers()
		{
			List<IResourceLevelObserver> list = new List<IResourceLevelObserver>();
			if ((this.observingComponents & ResourceObservingComponents.BootScanner) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("BootScanner"))
			{
				list.Add(new BootScannerResourceLevelObserver(Components.BootScanner));
			}
			IStartableTransportComponent contentAggregator;
			if ((this.observingComponents & ResourceObservingComponents.ContentAggregator) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("ContentAggregator") && Components.TryGetAggregator(out contentAggregator))
			{
				list.Add(new ContentAggregatorResourceLevelObserver(contentAggregator));
			}
			if ((this.observingComponents & ResourceObservingComponents.EnhancedDns) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("EnhancedDns"))
			{
				list.Add(new EnhancedDnsResourceLevelObserver(Components.EnhancedDns, this.componentsWrapper));
			}
			if ((this.observingComponents & ResourceObservingComponents.IsMemberOfResolver) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("IsMemberOfResolver"))
			{
				list.Add(new IsMofRResourceLevelObserver(Components.TransportIsMemberOfResolverComponent, this.componentsWrapper));
			}
			if ((this.observingComponents & ResourceObservingComponents.MessageResubmission) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("MessageResubmission"))
			{
				list.Add(new MessageResubmissionResourceLevelObserver(Components.MessageResubmissionComponent));
			}
			if ((this.observingComponents & ResourceObservingComponents.PickUp) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("Pickup"))
			{
				list.Add(new PickupResourceLevelObserver(Components.PickupComponent));
			}
			if ((this.observingComponents & ResourceObservingComponents.RemoteDelivery) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("RemoteDelivery"))
			{
				list.Add(new RemoteDeliveryResourceLevelObserver(Components.RemoteDeliveryComponent, this.messagingDatabaseComponent.Database.DataSource.DatabasePath, this.resourceThrottlingConfig.DehydrateMessagesUnderMemoryPressure));
			}
			if ((this.observingComponents & ResourceObservingComponents.ShadowRedundancy) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("ShadowRedundancy"))
			{
				list.Add(new ShadowRedundancyResourceLevelObserver(Components.ShadowRedundancyComponent));
			}
			if ((this.observingComponents & ResourceObservingComponents.SmtpIn) > ResourceObservingComponents.None && !this.resourceThrottlingConfig.DisabledResourceLevelObservers.Contains("SmtpIn"))
			{
				ResourceManagerConfiguration.ThrottlingControllerConfiguration config = new ResourceManagerConfiguration.ThrottlingControllerConfiguration(this.resourceThrottlingConfig.BaseThrottlingDelayInterval, this.resourceThrottlingConfig.StartThrottlingDelayInterval, this.resourceThrottlingConfig.StepThrottlingDelayInterval, this.resourceThrottlingConfig.MaxThrottlingDelayInterval);
				ThrottlingController throttlingController = new ThrottlingController(ExTraceGlobals.ResourceManagerTracer, config);
				list.Add(new SmtpInResourceLevelObserver(Components.SmtpInComponent, throttlingController, this.componentsWrapper));
			}
			return list;
		}

		private IResourceMeter CreateResourceMeter(ResourceIdentifier resource)
		{
			PressureTransitions arg = this.pressureTransitionsForResources[resource];
			Func<ResourceIdentifier, PressureTransitions, IResourceMeter> func;
			if (!this.resourceMeterCreators.TryGetValue(resource.ToString(), out func) && !this.resourceMeterCreators.TryGetValue(resource.Name, out func))
			{
				throw new InvalidOperationException("Resource Meter Creator is not found for " + resource);
			}
			return func(resource, arg);
		}

		private IResourceMeter CreatePrivateBytesResourceMeter(ResourceIdentifier resource, PressureTransitions pressureTransitions)
		{
			IResourceMeter rawResourceMeter = new PrivateBytesResourceMeter(pressureTransitions, ResourceMeteringConfig.TotalPhysicalMemory);
			return new StabilizedResourceMeter(rawResourceMeter, this.resourceMeteringConfig.PrivateBytesStabilizationSamples);
		}

		private IResourceMeter CreateDatabaseUsedSpaceResourceMeter(ResourceIdentifier resource, PressureTransitions pressureTransitions)
		{
			IMeterableJetDataSource meterableDataSource = MeterableJetDataSourceFactory.CreateMeterableDataSource(this.messagingDatabaseComponent.Database.DataSource);
			return new DatabaseUsedSpaceMeter(meterableDataSource, pressureTransitions);
		}

		private IResourceMeter CreateSubmissionQueueLengthResourceMeter(ResourceIdentifier resource, PressureTransitions pressureTransitions)
		{
			IMeterableQueue meterableQueue = MeterableQueueFactory.CreateMeterableQueue("SubmissionQueue", this.categorizerComponent.SubmitMessageQueue);
			IResourceMeter rawResourceMeter = new QueueLengthResourceMeter(meterableQueue, pressureTransitions);
			return new StabilizedResourceMeter(rawResourceMeter, this.resourceMeteringConfig.SubmissionQueueStabilizationSamples);
		}

		private IResourceMeter CreateSystemMemoryResourceMeter(ResourceIdentifier resource, PressureTransitions pressureTransitions)
		{
			return new SystemMemoryResourceMeter(pressureTransitions);
		}

		private IResourceMeter CreateUsedVersionBucketsResourceMeter(ResourceIdentifier resource, PressureTransitions pressureTransitions)
		{
			IMeterableJetDataSource meterableDataSourcee = MeterableJetDataSourceFactory.CreateMeterableDataSource(this.messagingDatabaseComponent.Database.DataSource);
			IResourceMeter rawResourceMeter = new UsedVersionBucketsResourceMeter(meterableDataSourcee, pressureTransitions);
			return new StabilizedResourceMeter(rawResourceMeter, this.resourceMeteringConfig.VersionBucketsStabilizationSamples);
		}

		private IResourceMeter CreateUsedDiskSpaceMeter(ResourceIdentifier resource, PressureTransitions pressureTransitions)
		{
			return new UsedDiskSpaceResourceMeter(resource.InstanceName, pressureTransitions);
		}

		private string GetBackPressureDescription()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			if (!this.IsResourceTrackingEnabled)
			{
				return string.Empty;
			}
			foreach (ResourceUse resourceUse in this.resourceTracker.ResourceUses)
			{
				if (resourceUse.CurrentUseLevel == UseLevel.Low)
				{
					stringBuilder2.AppendLine(this.localizedResourceNames[resourceUse.Resource]);
				}
				else
				{
					stringBuilder3.AppendLine(this.localizedResourceNames[resourceUse.Resource]);
				}
			}
			if (stringBuilder3.Length > 0)
			{
				stringBuilder.AppendLine(Strings.ResourcesInAboveNormalPressure(stringBuilder3.ToString()));
			}
			StringBuilder stringBuilder4 = new StringBuilder();
			if (this.resourceThrottlingConfig.IsResourceThrottlingEnabled)
			{
				foreach (IResourceLevelObserver resourceLevelObserver in this.resourceLevelMediator.ResourceLevelObservers)
				{
					if (resourceLevelObserver.Paused)
					{
						stringBuilder4.AppendLine(this.componentsStateMap[resourceLevelObserver.Name + resourceLevelObserver.SubStatus]);
					}
				}
				if (stringBuilder4.Length > 0)
				{
					stringBuilder.AppendLine(Strings.ComponentsDisabledByBackPressure(stringBuilder4.ToString()));
				}
				else
				{
					stringBuilder.AppendLine(Strings.ComponentsDisabledNone);
				}
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder.AppendLine(Strings.ResourcesInNormalPressure(stringBuilder2.ToString()));
			}
			return stringBuilder.ToString();
		}

		void ITransportComponent.Unload()
		{
			if (!this.IsResourceTrackingEnabled)
			{
				return;
			}
			this.ThrowIfNotLoaded();
			this.statusTimer.Dispose(true);
			this.cancellationTokenSource.Cancel();
			try
			{
				this.resourceTrackingTask.Wait();
			}
			catch (OperationCanceledException)
			{
			}
			catch (AggregateException ex)
			{
				if (!(ex.InnerException is OperationCanceledException))
				{
					throw;
				}
			}
			this.resourceLevelMediator = null;
			this.resourceTracker = null;
		}

		string ITransportComponent.OnUnhandledException(Exception e)
		{
			if (this.resourceTracker != null)
			{
				return "ResourceThrottlingComponent is loaded.";
			}
			return "ResourceThrottlingComponent is not loaded.";
		}

		private void ThrowIfNotLoaded()
		{
			if (this.resourceTracker == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ResourceThrottlingComponent is not loaded.", new object[0]));
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "ResourceThrottling";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool verbose = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			XElement xelement = new XElement("ResourceThrottling");
			if (this.resourceMeteringConfig.IsResourceTrackingEnabled)
			{
				xelement.Add(this.resourceTracker.GetDiagnosticInfo(verbose));
				if (this.resourceThrottlingConfig.IsResourceThrottlingEnabled)
				{
					xelement.Add(this.resourceLevelMediator.GetDiagnosticInfo(verbose));
				}
			}
			return xelement;
		}

		private const int SustainedBackpressureSamplePercent = 85;

		private const string ComponentName = "ResourceThrottling";

		private readonly ResourceMeteringConfig resourceMeteringConfig;

		private readonly ResourceThrottlingConfig resourceThrottlingConfig;

		private MessagingDatabaseComponent messagingDatabaseComponent;

		private readonly CategorizerComponent categorizerComponent;

		private readonly ITransportConfiguration configComponent;

		private readonly ResourceManagerResources resourcesToMonitor;

		private readonly ResourceObservingComponents observingComponents;

		private readonly IComponentsWrapper componentsWrapper;

		private readonly Dictionary<string, Func<ResourceIdentifier, PressureTransitions, IResourceMeter>> resourceMeterCreators = new Dictionary<string, Func<ResourceIdentifier, PressureTransitions, IResourceMeter>>();

		private ResourceTracker resourceTracker;

		private ResourceLevelMediator resourceLevelMediator;

		private Task resourceTrackingTask;

		private CancellationTokenSource cancellationTokenSource;

		private Dictionary<ResourceIdentifier, PressureTransitions> pressureTransitionsForResources;

		private DateTime statusLastPublishTime;

		private List<ResourceIdentifier> meteredResources;

		private GuardedTimer statusTimer;

		private ResourceSampleStabilizer sustainedBackPressureStabilizer;

		private ResourceThrottlingPerfCountersInstance perfCountersInstance;

		private DateTime backPressureStartTime = DateTime.MaxValue;

		private Dictionary<string, string> componentsStateMap = new Dictionary<string, string>
		{
			{
				"BootScanner",
				Strings.BootScannerComponent
			},
			{
				"ContentAggregator",
				Strings.ContentAggregationComponent
			},
			{
				"MessageResubmission",
				Strings.MessageResubmissionComponentBanner
			},
			{
				"Pickup",
				Strings.InboundMailSubmissionFromPickupDirectoryComponent + Environment.NewLine + Strings.InboundMailSubmissionFromReplayDirectoryComponent
			},
			{
				"RemoteDelivery",
				Strings.OutboundMailDeliveryToRemoteDomainsComponent
			},
			{
				"ShadowRedundancy",
				Strings.ShadowRedundancyComponentBanner
			},
			{
				"SmtpIn",
				Strings.InboundMailSubmissionFromInternetComponent
			},
			{
				"SmtpInRejecting Submissions",
				Strings.InboundMailSubmissionFromHubsComponent
			}
		};

		private readonly Dictionary<ResourceIdentifier, string> localizedResourceNames = new Dictionary<ResourceIdentifier, string>();

		private readonly ResourceManagerEventLogger eventLogger = new ResourceManagerEventLogger();

		private readonly ResourceIdentifier privateBytesResource = new ResourceIdentifier("PrivateBytes", "");

		private TimeSpan maxResourceMeterCallDuration;

		private TimeSpan maxResourceObserverCallDuration;
	}
}
