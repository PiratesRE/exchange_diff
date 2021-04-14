using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Cmdlet("Get", "O365ClientOSDetailReport")]
	[OutputType(new Type[]
	{
		typeof(ClientSoftwareOSDetailReport)
	})]
	public sealed class GetClientOSDetail : TenantReportBase<ClientSoftwareOSDetailReport>
	{
		[Parameter(Mandatory = false)]
		public string OperatingSystem { get; set; }

		[Parameter(Mandatory = false)]
		public string OperatingSystemVersion { get; set; }

		[Parameter(Mandatory = false)]
		public string WindowsLiveID { get; set; }

		protected override void ProcessNonPipelineParameter()
		{
			base.ProcessNonPipelineParameter();
			if (this.OperatingSystem != null || this.OperatingSystemVersion != null || this.WindowsLiveID != null)
			{
				base.AddQueryDecorator(new WhereDecorator<ClientSoftwareOSDetailReport>(base.TaskContext)
				{
					Predicate = ((ClientSoftwareOSDetailReport report) => ((this.OperatingSystem == null) ? true : this.OperatingSystem.Equals(report.Name)) && ((this.OperatingSystemVersion == null) ? true : this.OperatingSystemVersion.Equals(report.Version)) && ((this.WindowsLiveID == null) ? true : this.WindowsLiveID.Equals(report.UPN)))
				});
			}
		}
	}
}
