using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DLPISVService : DataSourceService
	{
		public PowerShellResults ProcessUpload(DLPPolicyUploadParameters parameters)
		{
			parameters.FaultIfNull();
			if (parameters is DLPNewPolicyUploadParameters)
			{
				return base.Invoke(new PSCommand().AddCommand("New-DLPPolicy"), Identity.FromExecutingUserId(), parameters);
			}
			return null;
		}
	}
}
