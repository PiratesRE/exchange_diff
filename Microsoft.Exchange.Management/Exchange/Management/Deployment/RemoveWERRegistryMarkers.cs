using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "WERRegistryMarkers")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public sealed class RemoveWERRegistryMarkers : ManageWERRegistryMarkers
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.DeleteRegistryMarkers();
			TaskLogger.LogExit();
		}
	}
}
