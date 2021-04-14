using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "EnvironmentVariable")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public sealed class RemoveEnvironmentVariable : ManageEnvironmentVariable
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.SetVariable(base.Name, null, base.Target);
			TaskLogger.LogExit();
		}
	}
}
