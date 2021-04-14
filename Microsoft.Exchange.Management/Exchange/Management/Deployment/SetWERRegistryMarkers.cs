using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "WERRegistryMarkers")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public sealed class SetWERRegistryMarkers : ManageWERRegistryMarkers
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.WriteRegistryMarkers();
			TaskLogger.LogExit();
		}
	}
}
