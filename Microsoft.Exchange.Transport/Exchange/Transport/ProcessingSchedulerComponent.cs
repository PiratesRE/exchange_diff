using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.Scheduler;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Scheduler.Processing;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport
{
	internal class ProcessingSchedulerComponent : IProcessingSchedulerComponent, ITransportComponent, IDiagnosable
	{
		public void SetLoadTimeDependencies(ITransportConfiguration transportConfiguration, IMessageDepotComponent messageDepotComponent, IMessageProcessor messageProcessor, IMessagingDatabaseComponent messagingDatabaseComponent)
		{
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			ArgumentValidator.ThrowIfNull("messageDepotComponent", messageDepotComponent);
			ArgumentValidator.ThrowIfNull("messageProcessor", messageProcessor);
			ArgumentValidator.ThrowIfNull("messagingDatabaseComponent", messagingDatabaseComponent);
			this.transportConfiguration = transportConfiguration;
			this.messageDepotComponent = messageDepotComponent;
			this.messageProcessor = messageProcessor;
			this.messagingDatabaseComponent = messagingDatabaseComponent;
		}

		public void Load()
		{
			if (!this.messageDepotComponent.Enabled)
			{
				return;
			}
			IQueueFactory queueFactory = this.GetQueueFactory();
			ISchedulerMetering meteringInstance = this.GetMeteringInstance();
			IQueueLogWriter queueLogWriter = this.GetQueueLogWriter();
			ISchedulerDiagnostics schedulerDiagnostics = new SchedulerDiagnostics(TimeSpan.FromMinutes(1.0), meteringInstance, queueLogWriter);
			ISchedulerThrottler throttlerInstance = this.GetThrottlerInstance(meteringInstance);
			IScopedQueuesManager scopedQueuesManager = this.GetScopedQueuesManager(throttlerInstance, queueFactory, schedulerDiagnostics);
			this.scheduler = new ProcessingScheduler(CatScheduler.MaxExecutingJobs, this.messageProcessor, meteringInstance, queueFactory, throttlerInstance, scopedQueuesManager, schedulerDiagnostics, false);
			this.schedulerAdmin = new ProcessingSchedulerAdminWrapper(this.scheduler, this.messagingDatabaseComponent);
			this.SetupLatencyTracker();
			this.diagnosticPublisher = new SchedulerDiagnosticPublisher(this.scheduler);
			this.refreshTimer = new GuardedTimer(new TimerCallback(this.TimedUpdate), null, ProcessingSchedulerComponent.RefreshTimeInterval);
		}

		public void Unload()
		{
			if (this.refreshTimer != null)
			{
				this.refreshTimer.Dispose(true);
				this.refreshTimer = null;
			}
			if (this.scheduler != null)
			{
				this.scheduler.Shutdown(-1);
			}
		}

		public string OnUnhandledException(Exception e)
		{
			return string.Empty;
		}

		public IProcessingScheduler ProcessingScheduler
		{
			get
			{
				return this.scheduler;
			}
		}

		public IProcessingSchedulerAdmin ProcessingSchedulerAdmin
		{
			get
			{
				return this.schedulerAdmin;
			}
		}

		public void Pause()
		{
			this.scheduler.Pause();
		}

		public void Resume()
		{
			this.scheduler.Resume();
		}

		public string GetDiagnosticComponentName()
		{
			return "ProcessingScheduler";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			if (this.scheduler != null)
			{
				SchedulerDiagnosticsInfo diagnosticsInfo = this.scheduler.GetDiagnosticsInfo();
				xelement.Add(new XElement("Enabled", true));
				xelement.Add(new XElement("Running", this.scheduler.IsRunning));
				xelement.Add(new XElement("Received", diagnosticsInfo.Received));
				xelement.Add(new XElement("Throttled", diagnosticsInfo.Throttled));
				xelement.Add(new XElement("Dispatched", diagnosticsInfo.Dispatched));
				xelement.Add(new XElement("Processed", diagnosticsInfo.Processed));
				xelement.Add(new XElement("ReceiveRate", diagnosticsInfo.ReceiveRate));
				xelement.Add(new XElement("ThrottleRate", diagnosticsInfo.ThrottleRate));
				xelement.Add(new XElement("DispatchRate", diagnosticsInfo.DispatchRate));
				xelement.Add(new XElement("ProcessRate", diagnosticsInfo.ProcessRate));
				xelement.Add(new XElement("ProcessingVelocity", diagnosticsInfo.ProcessingVelocity));
				xelement.Add(new XElement("ScopedQueuesCreatedRate", diagnosticsInfo.ScopedQueuesCreatedRate));
				xelement.Add(new XElement("ScopedQueuesDestroyedRate", diagnosticsInfo.ScopedQueuesDestroyedRate));
				xelement.Add(new XElement("OldestLockTimeStamp", diagnosticsInfo.OldestLockTimeStamp));
				xelement.Add(new XElement("OldestScopedQueueCreateTime", diagnosticsInfo.OldestScopedQueueCreateTime));
			}
			else
			{
				xelement.Add(new XElement("Enabled", false));
			}
			return xelement;
		}

		private void SetupLatencyTracker()
		{
			this.scheduler.SubscribeToEvent(SchedulingState.Received, new SchedulingEventHandler(this.TrackLatency));
			this.scheduler.SubscribeToEvent(SchedulingState.Scoped, new SchedulingEventHandler(this.TrackLatency));
			this.scheduler.SubscribeToEvent(SchedulingState.Unscoped, new SchedulingEventHandler(this.TrackLatency));
			this.scheduler.SubscribeToEvent(SchedulingState.Dispatched, new SchedulingEventHandler(this.TrackLatency));
		}

		private void TrackLatency(SchedulingState state, ISchedulableMessage message)
		{
			TransportMailItem transportMailItem;
			try
			{
				IMessageDepotItemWrapper messageDepotItemWrapper = this.GetMessageDepot().Get(message.Id);
				transportMailItem = (messageDepotItemWrapper.Item.MessageObject as TransportMailItem);
			}
			catch (ItemNotFoundException)
			{
				return;
			}
			if (transportMailItem != null)
			{
				switch (state)
				{
				case SchedulingState.Received:
					LatencyTracker.BeginTrackLatency(LatencyComponent.ProcessingScheduler, transportMailItem.LatencyTracker);
					return;
				case SchedulingState.Scoped:
					LatencyTracker.BeginTrackLatency(LatencyComponent.ProcessingSchedulerScoped, transportMailItem.LatencyTracker);
					return;
				case SchedulingState.Unscoped:
					LatencyTracker.EndTrackLatency(LatencyComponent.ProcessingSchedulerScoped, transportMailItem.LatencyTracker);
					return;
				case SchedulingState.Dispatched:
					LatencyTracker.EndTrackLatency(LatencyComponent.ProcessingScheduler, transportMailItem.LatencyTracker);
					break;
				default:
					return;
				}
			}
		}

		private IMessageDepot GetMessageDepot()
		{
			return this.messageDepotComponent.MessageDepot;
		}

		private ISchedulerMetering GetMeteringInstance()
		{
			return new InMemorySchedulerMetering(TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(10.0));
		}

		private ISchedulerThrottler GetThrottlerInstance(ISchedulerMetering metering)
		{
			IThrottlingPolicy[] policies = new IThrottlingPolicy[]
			{
				new OutstandingJobsPolicy(CatScheduler.MaxExecutingJobs, 0.7),
				new MemoryUsagePolicy(10000000L, 0.7),
				new ProcessingTicksPolicy(TimeSpan.FromMinutes(15.0).Ticks)
			};
			MessageScopeType[] array = new MessageScopeType[1];
			MessageScopeType[] relevantTypes = array;
			return new PolicyBasedThrottler(policies, relevantTypes, metering);
		}

		private IQueueFactory GetQueueFactory()
		{
			return new PriorityQueueFactory(new Dictionary<DeliveryPriority, int>
			{
				{
					DeliveryPriority.High,
					5
				},
				{
					DeliveryPriority.Normal,
					3
				},
				{
					DeliveryPriority.Low,
					2
				},
				{
					DeliveryPriority.None,
					1
				}
			});
		}

		private IScopedQueuesManager GetScopedQueuesManager(ISchedulerThrottler throttler, IQueueFactory queueFactory, ISchedulerDiagnostics diagnostics)
		{
			return new ScopedQueuesManager(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(5.0), queueFactory, throttler, diagnostics, null);
		}

		private IQueueLogWriter GetQueueLogWriter()
		{
			Server transportServer = this.transportConfiguration.LocalServer.TransportServer;
			if (transportServer.ProcessingSchedulerLogEnabled && transportServer.ProcessingSchedulerLogPath != null && !string.IsNullOrEmpty(transportServer.ProcessingSchedulerLogPath.PathName))
			{
				return new AsyncQueueLogWriter("Microsoft Exchange Server", "Processing Scheduler Log", "ProcessingScheduler", "ProcessingScheduler", transportServer.ProcessingSchedulerLogPath.PathName, transportServer.ProcessingSchedulerLogMaxAge, TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(60.0), (long)(transportServer.ProcessingSchedulerLogMaxDirectorySize.IsUnlimited ? 0UL : transportServer.ProcessingSchedulerLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.ProcessingSchedulerLogMaxFileSize.IsUnlimited ? 0UL : transportServer.ProcessingSchedulerLogMaxFileSize.Value.ToBytes()), (int)ByteQuantifiedSize.FromKB(64UL).ToBytes());
			}
			return new NoOpQueueLogWriter();
		}

		private void TimedUpdate(object state)
		{
			this.diagnosticPublisher.Publish();
		}

		private static readonly TimeSpan RefreshTimeInterval = TimeSpan.FromSeconds(10.0);

		private IMessageProcessor messageProcessor;

		private ProcessingScheduler scheduler;

		private IProcessingSchedulerAdmin schedulerAdmin;

		private SchedulerDiagnosticPublisher diagnosticPublisher;

		private IMessageDepotComponent messageDepotComponent;

		private IMessagingDatabaseComponent messagingDatabaseComponent;

		private ITransportConfiguration transportConfiguration;

		private GuardedTimer refreshTimer;
	}
}
