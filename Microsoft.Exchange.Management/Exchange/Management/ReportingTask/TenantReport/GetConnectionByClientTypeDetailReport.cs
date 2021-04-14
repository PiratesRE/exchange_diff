using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(ConnectionByClientTypeDetailReport)
	})]
	[Cmdlet("Get", "ConnectionByClientTypeDetailReport")]
	public sealed class GetConnectionByClientTypeDetailReport : TenantReportBase<ConnectionByClientTypeDetailReport>
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
			ReportType key = ReportType.Daily;
			if (base.Fields.IsModified("ReportType"))
			{
				key = this.ReportType;
			}
			this.viewName = GetConnectionByClientTypeDetailReport.ReportTypeMapping[key];
		}

		private const string ReportTypeKey = "ReportType";

		private static readonly Dictionary<ReportType, string> ReportTypeMapping = new Dictionary<ReportType, string>
		{
			{
				ReportType.Daily,
				"dbo.ConnectionByClientTypeDetailDaily"
			},
			{
				ReportType.Weekly,
				"dbo.ConnectionByClientTypeDetailWeekly"
			},
			{
				ReportType.Monthly,
				"dbo.ConnectionByClientTypeDetailMonthly"
			},
			{
				ReportType.Yearly,
				"dbo.ConnectionByClientTypeDetailYearly"
			}
		};

		private string viewName;
	}
}
