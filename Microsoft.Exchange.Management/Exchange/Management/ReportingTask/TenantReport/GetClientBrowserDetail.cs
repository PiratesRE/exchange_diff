using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "O365ClientBrowserDetailReport")]
	[OutputType(new Type[]
	{
		typeof(ClientSoftwareBrowserDetailReport)
	})]
	public sealed class GetClientBrowserDetail : TenantReportBase<ClientSoftwareBrowserDetailReport>
	{
		[Parameter(Mandatory = false)]
		public string Browser { get; set; }

		[Parameter(Mandatory = false)]
		public string BrowserVersion { get; set; }

		[Parameter(Mandatory = false)]
		public string WindowsLiveID { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			if (this.Browser != null || this.BrowserVersion != null || this.WindowsLiveID != null)
			{
				base.AddQueryDecorator(new WhereDecorator<ClientSoftwareBrowserDetailReport>(base.TaskContext)
				{
					Predicate = ((ClientSoftwareBrowserDetailReport report) => ((this.Browser == null) ? true : this.Browser.Equals(report.Name)) && ((this.BrowserVersion == null) ? true : this.BrowserVersion.Equals(report.Version)) && ((this.WindowsLiveID == null) ? true : this.WindowsLiveID.Equals(report.UPN)))
				});
			}
		}
	}
}
