using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "SPOSkyDriveProDeployedReport")]
	[OutputType(new Type[]
	{
		typeof(SPOSkyDriveProDeployedReport)
	})]
	public sealed class GetSPOSkyDriveProDeployedReport : TenantReportBase<SPOSkyDriveProDeployedReport>
	{
		[Parameter(Mandatory = false)]
		public ReportType ReportType
		{
			get
			{
				return (ReportType)base.Fields["ReportType"];
			}
			set
			{
				base.Fields["ReportType"] = value;
			}
		}

		protected override string ViewName
		{
			get
			{
				return this.viewName;
			}
		}

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			this.ValidateReportType();
		}

		private void ValidateReportType()
		{
			ReportType key = ReportType.Weekly;
			if (base.Fields.IsModified("ReportType"))
			{
				key = this.ReportType;
			}
			this.viewName = GetSPOSkyDriveProDeployedReport.ReportTypeMapping[key];
		}

		private const string ReportTypeKey = "ReportType";

		private static readonly Dictionary<ReportType, string> ReportTypeMapping = new Dictionary<ReportType, string>
		{
			{
				ReportType.Weekly,
				"dbo.SPOSkyDriveProDeployedWeekly"
			},
			{
				ReportType.Monthly,
				"dbo.SPOSkyDriveProDeployedMonthly"
			}
		};

		private string viewName;
	}
}
