using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(ConnectionByClientTypeReport)
	})]
	[Cmdlet("Get", "ConnectionByClientTypeReport")]
	public sealed class GetConnectionByClientTypeReport : TenantReportBase<ConnectionByClientTypeReport>
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
			this.viewName = GetConnectionByClientTypeReport.ReportTypeMapping[key];
		}

		private const string ReportTypeKey = "ReportType";

		private static readonly Dictionary<ReportType, string> ReportTypeMapping = new Dictionary<ReportType, string>
		{
			{
				ReportType.Daily,
				"dbo.ConnectionByClientTypeDaily"
			},
			{
				ReportType.Weekly,
				"dbo.ConnectionByClientTypeWeekly"
			},
			{
				ReportType.Monthly,
				"dbo.ConnectionByClientTypeMonthly"
			},
			{
				ReportType.Yearly,
				"dbo.ConnectionByClientTypeYearly"
			}
		};

		private string viewName;
	}
}
