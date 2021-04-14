using System;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class GetMessageTrackingReportDetailParameters : GetMessageTrackingReportParameters
	{
		public string RecipientPathFilter
		{
			get
			{
				return (string)base["RecipientPathFilter"];
			}
			set
			{
				base["RecipientPathFilter"] = value;
			}
		}

		public ReportTemplate ReportTemplate
		{
			get
			{
				return (ReportTemplate)base["ReportTemplate"];
			}
			set
			{
				base["ReportTemplate"] = value;
			}
		}

		public new const string RbacParameters = "?Identity&ReportTemplate&RecipientPathFilter";
	}
}
