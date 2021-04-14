using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class WlmResource : ResourceBase
	{
		public WlmResource(WorkloadType workloadType)
		{
			this.WlmWorkloadType = workloadType;
			this.ResourceGuid = Guid.Empty;
			base.ConfigContext = new GenericSettingsContext("WorkloadType", this.WlmWorkloadType.ToString(), base.ConfigContext);
		}

		public WorkloadType WlmWorkloadType { get; private set; }

		public Guid ResourceGuid { get; protected set; }

		public string WlmWorkloadTypeSuffix
		{
			get
			{
				WorkloadType wlmWorkloadType = this.WlmWorkloadType;
				if (wlmWorkloadType == WorkloadType.MailboxReplicationServiceHighPriority)
				{
					return "-HiPri";
				}
				switch (wlmWorkloadType)
				{
				case WorkloadType.MailboxReplicationServiceInternalMaintenance:
					return "-IM";
				case WorkloadType.MailboxReplicationServiceInteractive:
					return "-I";
				default:
					return string.Empty;
				}
			}
		}

		public WorkloadClassification WorkloadClassification
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.GetObject<IWorkloadSettings>(this.WlmWorkloadType, new object[0]).Classification;
			}
		}

		public int DynamicCapacity
		{
			get
			{
				int num = this.StaticCapacity;
				foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in this.healthMonitors)
				{
					if (wlmResourceHealthMonitor.DynamicCapacity < num)
					{
						num = wlmResourceHealthMonitor.DynamicCapacity;
					}
				}
				return num;
			}
		}

		public override bool IsUnhealthy
		{
			get
			{
				foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in this.healthMonitors)
				{
					if (wlmResourceHealthMonitor.IsUnhealthy)
					{
						return true;
					}
				}
				return base.IsUnhealthy;
			}
		}

		public abstract List<WlmResourceHealthMonitor> GetWlmResources();

		public void UpdateHealthState(bool logHealthState)
		{
			this.InitializeMonitors();
			foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in this.healthMonitors)
			{
				wlmResourceHealthMonitor.UpdateHealthState(logHealthState);
			}
		}

		protected override void VerifyDynamicCapacity(ReservationBase reservation)
		{
			this.InitializeMonitors();
			foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in this.healthMonitors)
			{
				wlmResourceHealthMonitor.VerifyDynamicCapacity(reservation);
			}
		}

		protected override void AddReservation(ReservationBase reservation)
		{
			foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in this.healthMonitors)
			{
				wlmResourceHealthMonitor.AddReservation(reservation);
			}
			base.AddReservation(reservation);
		}

		protected override XElement GetDiagnosticInfoInternal(MRSDiagnosticArgument arguments)
		{
			this.InitializeMonitors();
			ResourceDiagnosticInfoXML resourceDiagnosticInfoXML = new ResourceDiagnosticInfoXML
			{
				ResourceName = this.ResourceName,
				ResourceGuid = this.ResourceGuid,
				ResourceType = this.ResourceType,
				StaticCapacity = this.StaticCapacity,
				DynamicCapacity = this.DynamicCapacity,
				Utilization = base.Utilization,
				IsUnhealthy = this.IsUnhealthy,
				WlmWorkloadType = this.WlmWorkloadType.ToString(),
				TransferRatePerMin = this.TransferRate.GetValue()
			};
			if (this.healthMonitors.Count > 0)
			{
				resourceDiagnosticInfoXML.WlmResourceHealthMonitors = new List<WlmResourceHealthMonitorDiagnosticInfoXML>();
				foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in this.healthMonitors)
				{
					WlmResourceHealthMonitorDiagnosticInfoXML wlmResourceHealthMonitorDiagnosticInfoXML = wlmResourceHealthMonitor.PopulateDiagnosticInfo(arguments);
					if (wlmResourceHealthMonitorDiagnosticInfoXML != null)
					{
						resourceDiagnosticInfoXML.WlmResourceHealthMonitors.Add(wlmResourceHealthMonitorDiagnosticInfoXML);
					}
				}
			}
			return resourceDiagnosticInfoXML.ToDiagnosticInfo(null);
		}

		private void InitializeMonitors()
		{
			if (this.healthMonitors != null)
			{
				return;
			}
			this.healthMonitors = this.GetWlmResources();
		}

		private List<WlmResourceHealthMonitor> healthMonitors;
	}
}
