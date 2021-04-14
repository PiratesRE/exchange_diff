using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ADServerSettings
{
	[OutputType(new Type[]
	{
		typeof(RunspaceServerSettingsPresentationObject)
	})]
	[Cmdlet("Get", "AdServerSettings")]
	public sealed class GetAdServerSettings : Task
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.ServerSettings != null)
			{
				base.WriteObject(new RunspaceServerSettingsPresentationObject((RunspaceServerSettings)base.ServerSettings));
			}
		}
	}
}
