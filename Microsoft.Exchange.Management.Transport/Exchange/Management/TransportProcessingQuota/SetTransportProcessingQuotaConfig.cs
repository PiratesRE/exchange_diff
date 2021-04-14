using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	[Cmdlet("Set", "TransportProcessingQuotaConfig", SupportsShouldProcess = true)]
	public sealed class SetTransportProcessingQuotaConfig : TransportProcessingQuotaBaseTask
	{
		[Parameter(Mandatory = false)]
		public double? AmWeight { get; set; }

		[Parameter(Mandatory = false)]
		public double? AsWeight { get; set; }

		[Parameter(Mandatory = false)]
		public bool? CalculationEnabled { get; set; }

		[Parameter(Mandatory = false)]
		public int? CalculationFrequency { get; set; }

		[Parameter(Mandatory = false)]
		public int? CostThreshold { get; set; }

		[Parameter(Mandatory = false)]
		public double? EtrWeight { get; set; }

		[Parameter(Mandatory = false)]
		public bool? ThrottlingEnabled { get; set; }

		[Parameter(Mandatory = false)]
		public int? TimeWindow { get; set; }

		[Parameter(Mandatory = false)]
		public double? ThrottleFactor { get; set; }

		[Parameter(Mandatory = false)]
		public double? RelativeCostThreshold { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTransportProcessingConfig;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			TransportProcessingQuotaConfig transportThrottlingConfig = base.Session.GetTransportThrottlingConfig();
			if (this.AmWeight != null)
			{
				transportThrottlingConfig.AmWeight = this.AmWeight.Value;
			}
			if (this.AsWeight != null)
			{
				transportThrottlingConfig.AsWeight = this.AsWeight.Value;
			}
			if (this.CalculationEnabled != null)
			{
				transportThrottlingConfig.CalculationEnabled = this.CalculationEnabled.Value;
			}
			if (this.CalculationFrequency != null)
			{
				transportThrottlingConfig.CalculationFrequency = this.CalculationFrequency.Value;
			}
			if (this.CostThreshold != null)
			{
				transportThrottlingConfig.CostThreshold = this.CostThreshold.Value;
			}
			if (this.ThrottlingEnabled != null)
			{
				transportThrottlingConfig.ThrottlingEnabled = this.ThrottlingEnabled.Value;
			}
			if (this.EtrWeight != null)
			{
				transportThrottlingConfig.EtrWeight = this.EtrWeight.Value;
			}
			if (this.TimeWindow != null)
			{
				transportThrottlingConfig.TimeWindow = this.TimeWindow.Value;
			}
			if (this.ThrottleFactor != null)
			{
				transportThrottlingConfig.ThrottleFactor = this.ThrottleFactor.Value;
			}
			if (this.RelativeCostThreshold != null)
			{
				transportThrottlingConfig.RelativeCostThreshold = this.RelativeCostThreshold.Value;
			}
			base.Session.SetTransportThrottlingConfig(transportThrottlingConfig);
		}
	}
}
