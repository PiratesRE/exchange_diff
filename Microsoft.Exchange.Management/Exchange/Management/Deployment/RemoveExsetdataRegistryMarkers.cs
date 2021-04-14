using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ExsetdataRegistryMarkers")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveExsetdataRegistryMarkers : ManageExsetdataRegistryMarkers
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.DeleteRegistryMarkers();
			TaskLogger.LogExit();
		}
	}
}
