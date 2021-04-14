using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.FfoReporting;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DLPPolicyReportingService : DataSourceService
	{
		public PowerShellResults<MailTrafficPolicyReport> GetDLPTrafficData(DLPPolicyTrafficReportParameters parameters)
		{
			parameters.FaultIfNull();
			if (parameters != null)
			{
				return base.GetList<MailTrafficPolicyReport, DLPPolicyTrafficReportParameters>(new PSCommand().AddCommand("Get-MailTrafficPolicyReport"), parameters, null);
			}
			return null;
		}
	}
}
