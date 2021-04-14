using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "ExsetdataRegistryMarkers")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class SetExsetdataRegistryMarkers : ManageExsetdataRegistryMarkers
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.WriteRegistryMarkers();
			TaskLogger.LogExit();
		}
	}
}
