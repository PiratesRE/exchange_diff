using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(ExternalActivitySummaryReport)
	})]
	[Cmdlet("Get", "ExternalActivitySummaryReport")]
	public sealed class GetExternalActivitySummaryReport : TenantReportBase<ExternalActivitySummaryReport>
	{
		protected override DataMartType DataMartType
		{
			get
			{
				return DataMartType.TenantSecurity;
			}
		}

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
			ReportType key = ReportType.Daily;
			if (base.Fields.IsModified("ReportType"))
			{
				key = this.ReportType;
			}
			this.viewName = GetExternalActivitySummaryReport.ReportTypeMapping[key];
		}

		private const string ReportTypeKey = "ReportType";

		private static readonly Dictionary<ReportType, string> ReportTypeMapping = new Dictionary<ReportType, string>
		{
			{
				ReportType.Daily,
				"dbo.ExternalDomainSummaryDaily"
			},
			{
				ReportType.Weekly,
				"dbo.ExternalDomainSummaryWeekly"
			},
			{
				ReportType.Monthly,
				"dbo.ExternalDomainSummaryMonthly"
			}
		};

		private string viewName;
	}
}
