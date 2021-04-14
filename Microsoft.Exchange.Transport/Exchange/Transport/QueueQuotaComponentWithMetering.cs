using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaComponentWithMetering : IQueueQuotaComponent, ITransportComponent, IDiagnosable
	{
		public QueueQuotaComponentWithMetering() : this(() => DateTime.UtcNow)
		{
		}

		public QueueQuotaComponentWithMetering(Func<DateTime> currentTimeProvider)
		{
			this.currentTimeProvider = currentTimeProvider;
		}

		public void SetRunTimeDependencies(IQueueQuotaConfig config, IFlowControlLog log, IQueueQuotaComponentPerformanceCounters perfCounters, IProcessingQuotaComponent processingQuotaComponent, IQueueQuotaObservableComponent submissionQueue, IQueueQuotaObservableComponent deliveryQueue, ICountTracker<MeteredEntity, MeteredCount> metering)
		{
			this.config = config;
			this.queueQuota = new QueueQuotaImpl(config, log, perfCounters, processingQuotaComponent, metering, this.currentTimeProvider);
			if (submissionQueue != null)
			{
				submissionQueue.OnAcquire += delegate(TransportMailItem tmi)
				{
					this.queueQuota.TrackEnteringQueue(tmi, QueueQuotaResources.SubmissionQueueSize | QueueQuotaResources.TotalQueueSize);
				};
				submissionQueue.OnRelease += delegate(TransportMailItem tmi)
				{
					this.queueQuota.TrackExitingQueue(tmi, QueueQuotaResources.SubmissionQueueSize | QueueQuotaResources.TotalQueueSize);
				};
			}
			if (deliveryQueue != null)
			{
				deliveryQueue.OnAcquire += delegate(TransportMailItem tmi)
				{
					this.queueQuota.TrackEnteringQueue(tmi, QueueQuotaResources.TotalQueueSize);
				};
				deliveryQueue.OnRelease += delegate(TransportMailItem tmi)
				{
					this.queueQuota.TrackExitingQueue(tmi, QueueQuotaResources.TotalQueueSize);
				};
			}
		}

		public void TrackEnteringQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resources)
		{
			this.queueQuota.TrackEnteringQueue(mailItem, resources);
		}

		public void TrackExitingQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resources)
		{
			this.queueQuota.TrackExitingQueue(mailItem, resources);
		}

		public bool IsOrganizationOverQuota(string accountForest, Guid externalOrganizationId, string sender, out string reason)
		{
			return this.queueQuota.IsOrganizationOverQuota(accountForest, externalOrganizationId, sender, out reason);
		}

		public bool IsOrganizationOverWarning(string accountForest, Guid externalOrganizationId, string sender, QueueQuotaResources resource)
		{
			return this.queueQuota.IsOrganizationOverWarning(accountForest, externalOrganizationId, sender, resource);
		}

		public void TimedUpdate()
		{
			this.queueQuota.TimedUpdate();
		}

		internal bool IsOverQuota(string accountForest, Guid externalOrganizationId, string sender, out string reason, out QueueQuotaEntity? reasonEntity, out QueueQuotaResources? reasonResource)
		{
			return this.queueQuota.IsOverQuota(accountForest, externalOrganizationId, sender, out reason, out reasonEntity, out reasonResource);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "QueueQuota";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			xelement.SetAttributeValue("Version", "NewMetering");
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("Tenant:", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("Forest:", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = parameters.Argument.Equals("config", StringComparison.InvariantCultureIgnoreCase);
			bool flag5 = (!flag4 && !flag && !flag2) || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag)
			{
				xelement.Add(this.queueQuota.GetDiagnosticInfo());
			}
			if (flag2)
			{
				string text = parameters.Argument.Substring("Tenant:".Length);
				Guid externalOrganizationId;
				if (Guid.TryParse(text, out externalOrganizationId))
				{
					xelement.Add(this.queueQuota.GetDiagnosticInfo(externalOrganizationId));
				}
				else
				{
					xelement.Add(new XElement("Error", string.Format("Invalid external organization id {0} passed as argument. Expecting a Guid.", text)));
				}
			}
			if (flag3)
			{
				string accountForest = parameters.Argument.Substring("Forest:".Length);
				xelement.Add(this.queueQuota.GetDiagnosticInfo(accountForest));
			}
			if (flag4)
			{
				xelement.Add(TransportAppConfig.GetDiagnosticInfoForType(this.config));
			}
			if (flag5)
			{
				xelement.Add(new XElement("help", string.Format("Supported arguments: verbose, help, config, {0}'tenantID e.g.1afa2e80-0251-4521-8086-039fb2f9d8d6', {1}'forestFQDN e.g. nampr03a001.prod.outlook.com'.", "Tenant:", "Forest:")));
			}
			return xelement;
		}

		public void Load()
		{
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private const string DiagnosticsComponentName = "QueueQuota";

		private readonly Func<DateTime> currentTimeProvider;

		private QueueQuotaImpl queueQuota;

		private IQueueQuotaConfig config;
	}
}
