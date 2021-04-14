using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class WlmResourceHealthMonitor
	{
		public WlmResourceHealthMonitor(WlmResource owner, ResourceKey resourceKey)
		{
			this.Owner = owner;
			this.WlmResourceKey = resourceKey;
			this.healthTracker = new WlmHealthSLA();
			this.admissionControl = new DefaultAdmissionControl(this.WlmResourceKey, new RemoveResourceDelegate(this.ResetAdmissionControl), null, "MRS_WlmResourceHealthMonitor");
			this.configContext = new GenericSettingsContext("WlmHealthMonitor", this.WlmResourceKey.ToString(), null);
		}

		public WlmResource Owner { get; private set; }

		public ResourceKey WlmResourceKey { get; private set; }

		public ExPerformanceCounter ResourceHealthPerfCounter { get; protected set; }

		public ExPerformanceCounter DynamicCapacityPerfCounter { get; protected set; }

		public int DynamicCapacity
		{
			get
			{
				ResourceLoad currentLoad = this.GetCurrentLoad();
				int num;
				if (this.DynamicThrottlingDisabled)
				{
					num = this.Owner.StaticCapacity;
				}
				else
				{
					double num2;
					num = this.admissionControl.GetConcurrencyLimit(this.Owner.WorkloadClassification, out num2);
					int maxConcurrency = this.admissionControl.MaxConcurrency;
					if (num > maxConcurrency)
					{
						num = maxConcurrency;
					}
				}
				this.DynamicCapacityPerfCounter.RawValue = (long)num;
				this.healthTracker.AddSample(currentLoad.State, num);
				return num;
			}
		}

		public bool IsUnhealthy
		{
			get
			{
				switch (this.GetCurrentLoad().State)
				{
				case ResourceLoadState.Unknown:
					return this.Owner.Utilization > 0;
				case ResourceLoadState.Full:
					return this.Owner.Utilization == 0;
				case ResourceLoadState.Overloaded:
				case ResourceLoadState.Critical:
					return true;
				}
				return false;
			}
		}

		public bool IsDisabled
		{
			get
			{
				bool config;
				using (this.configContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<bool>("IgnoreHealthMonitor");
				}
				return config;
			}
		}

		public bool DynamicThrottlingDisabled
		{
			get
			{
				bool config;
				using (this.configContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<bool>("DisableDynamicThrottling");
				}
				return config;
			}
		}

		public void VerifyDynamicCapacity(ReservationBase reservation)
		{
			if (this.IsDisabled)
			{
				return;
			}
			ResourceLoad currentLoad = this.GetCurrentLoad();
			if (currentLoad.State == ResourceLoadState.Overloaded || currentLoad.State == ResourceLoadState.Critical)
			{
				throw new WlmResourceUnhealthyException(this.Owner.ResourceName, this.Owner.ResourceType, this.WlmResourceKey.ToString(), (int)this.WlmResourceKey.MetricType, currentLoad.LoadRatio, currentLoad.State.ToString(), (currentLoad.Metric != null) ? currentLoad.Metric.ToString() : "(null)");
			}
			if (!this.DynamicThrottlingDisabled && this.Owner.Utilization >= this.DynamicCapacity)
			{
				throw new WlmCapacityExceededReservationException(this.Owner.ResourceName, this.Owner.ResourceType, this.WlmResourceKey.ToString(), (int)this.WlmResourceKey.MetricType, this.DynamicCapacity);
			}
		}

		public void LogHealthState()
		{
			ResourceLoad currentLoad = this.GetCurrentLoad();
			WlmHealthCounters customHealthCounters = this.healthTracker.GetCustomHealthCounters();
			WLMResourceStatsData loggingStatsData = new WLMResourceStatsData
			{
				OwnerResourceName = this.Owner.ResourceName,
				OwnerResourceGuid = this.Owner.ResourceGuid,
				OwnerResourceType = this.Owner.ResourceType,
				WlmResourceKey = this.WlmResourceKey.ToString(),
				DynamicCapacity = (double)this.DynamicCapacity,
				LoadState = currentLoad.State.ToString(),
				LoadRatio = currentLoad.LoadRatio,
				Metric = ((currentLoad.Metric != null) ? currentLoad.Metric.ToString() : string.Empty),
				IsDisabled = (this.IsDisabled ? "true" : null),
				DynamicThrottingDisabled = (this.DynamicThrottlingDisabled ? "true" : null),
				TimeInterval = this.healthTracker.CustomTimeInterval,
				UnderloadedCount = customHealthCounters.UnderloadedCounter,
				FullCount = customHealthCounters.FullCounter,
				OverloadedCount = customHealthCounters.OverloadedCounter,
				CriticalCount = customHealthCounters.CriticalCounter,
				UnknownCount = customHealthCounters.UnknownCounter
			};
			WLMResourceStatsLog.Write(loggingStatsData);
		}

		public void UpdateHealthState(bool logHealthState)
		{
			int dynamicCapacity = this.DynamicCapacity;
			if (logHealthState)
			{
				this.LogHealthState();
			}
		}

		public void AddReservation(ReservationBase reservation)
		{
			if (!this.IsDisabled && !this.DynamicThrottlingDisabled)
			{
				IResourceAdmissionControl admissionControl = this.admissionControl;
				double num;
				if (!admissionControl.TryAcquire(this.Owner.WorkloadClassification, out num))
				{
					ResourceLoad currentLoad = this.GetCurrentLoad();
					throw new WlmResourceUnhealthyException(this.Owner.ResourceName, this.Owner.ResourceType, this.WlmResourceKey.ToString(), (int)this.WlmResourceKey.MetricType, currentLoad.LoadRatio, currentLoad.State.ToString(), (currentLoad.Metric != null) ? currentLoad.Metric.ToString() : "(null)");
				}
				reservation.AddReleaseAction(delegate(ReservationBase r)
				{
					this.ReleaseReservation(r, admissionControl);
				});
			}
		}

		public WlmResourceHealthMonitorDiagnosticInfoXML PopulateDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			if (arguments.HasArgument("unhealthy") && !this.IsUnhealthy)
			{
				return null;
			}
			ResourceLoad currentLoad = this.GetCurrentLoad();
			WlmResourceHealthMonitorDiagnosticInfoXML wlmResourceHealthMonitorDiagnosticInfoXML = new WlmResourceHealthMonitorDiagnosticInfoXML
			{
				WlmResourceKey = this.WlmResourceKey.ToString(),
				DynamicCapacity = (double)this.DynamicCapacity,
				LoadState = currentLoad.State.ToString(),
				LoadRatio = currentLoad.LoadRatio,
				Metric = ((currentLoad.Metric != null) ? currentLoad.Metric.ToString() : string.Empty),
				IsDisabled = (this.IsDisabled ? "true" : null),
				DynamicThrottingDisabled = (this.DynamicThrottlingDisabled ? "true" : null)
			};
			if (arguments.HasArgument("healthstats"))
			{
				wlmResourceHealthMonitorDiagnosticInfoXML.WlmHealthStatistics = this.healthTracker.GetStats();
			}
			return wlmResourceHealthMonitorDiagnosticInfoXML;
		}

		private ResourceLoad GetCurrentLoad()
		{
			ResourceLoad result;
			if (this.IsDisabled)
			{
				result = ResourceLoad.Zero;
			}
			else if (TestIntegration.Instance.AssumeWLMUnhealthyForReservations)
			{
				result = new ResourceLoad(12321.0, new int?(12321), null);
			}
			else
			{
				IResourceLoadMonitor resourceLoadMonitor = ResourceHealthMonitorManager.Singleton.Get(this.WlmResourceKey);
				if (resourceLoadMonitor == null)
				{
					result = new ResourceLoad(24642.0, new int?(24642), null);
				}
				else
				{
					result = resourceLoadMonitor.GetResourceLoad(this.Owner.WorkloadClassification, false, null);
				}
			}
			this.ResourceHealthPerfCounter.RawValue = (long)result.State;
			return result;
		}

		private void ResetAdmissionControl(ResourceKey key)
		{
			this.admissionControl = new DefaultAdmissionControl(this.WlmResourceKey, new RemoveResourceDelegate(this.ResetAdmissionControl), null, "MRS_WlmResourceHealthMonitor");
		}

		private void ReleaseReservation(ReservationBase reservation, IResourceAdmissionControl admissionControl)
		{
			try
			{
				admissionControl.Release(this.Owner.WorkloadClassification);
			}
			catch (NonOperationalAdmissionControlException ex)
			{
				MrsTracer.Common.Warning("Releasing a reservation from a non-operational AdmissionControl instance. Ignoring exception {0}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex, true)
				});
			}
		}

		private const string AdmissionControlOwner = "MRS_WlmResourceHealthMonitor";

		private WlmHealthSLA healthTracker;

		private DefaultAdmissionControl admissionControl;

		private GenericSettingsContext configContext;
	}
}
