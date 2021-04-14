using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[OutputType(new Type[]
	{
		typeof(CsActiveUserReport)
	})]
	[Cmdlet("Get", "CsActiveUserReport")]
	public sealed class GetCsActiveUserReport : TenantReportBase<CsActiveUserReport>
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
			this.viewName = GetCsActiveUserReport.ReportTypeMapping[key];
		}

		private const string ReportTypeKey = "ReportType";

		private static readonly Dictionary<ReportType, string> ReportTypeMapping = new Dictionary<ReportType, string>
		{
			{
				ReportType.Daily,
				"dbo.CsActiveUserDaily"
			},
			{
				ReportType.Weekly,
				"dbo.CsActiveUserWeekly"
			},
			{
				ReportType.Monthly,
				"dbo.CsActiveUserMonthly"
			},
			{
				ReportType.Yearly,
				"dbo.CsActiveUserYearly"
			}
		};

		private string viewName;
	}
}
