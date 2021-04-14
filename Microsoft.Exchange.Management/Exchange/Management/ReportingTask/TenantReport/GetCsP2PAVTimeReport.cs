using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Common;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "CsP2PAVTimeReport")]
	[OutputType(new Type[]
	{
		typeof(CsP2PAVTimeReport)
	})]
	public sealed class GetCsP2PAVTimeReport : TenantReportBase<CsP2PAVTimeReport>
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
			this.viewName = GetCsP2PAVTimeReport.ReportTypeMapping[key];
		}

		private const string ReportTypeKey = "ReportType";

		private static readonly Dictionary<ReportType, string> ReportTypeMapping = new Dictionary<ReportType, string>
		{
			{
				ReportType.Daily,
				"dbo.CsP2PAVTimeDaily"
			},
			{
				ReportType.Weekly,
				"dbo.CsP2PAVTimeWeekly"
			},
			{
				ReportType.Monthly,
				"dbo.CsP2PAVTimeMonthly"
			}
		};

		private string viewName;
	}
}
