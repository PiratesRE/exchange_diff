using System;
using System.Management.Automation;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management
{
	public abstract class GetHealthBase : PSCmdlet
	{
		[Alias(new string[]
		{
			"Server"
		})]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public string Identity { get; set; }

		[Parameter(Mandatory = false)]
		public string HealthSet { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter HaImpactingOnly { get; set; }
	}
}
