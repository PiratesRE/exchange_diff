using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Set", "TopologyMode")]
	public sealed class SetTopologyMode : Task
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADSession.SetAdminTopologyMode();
			if (ExchangePropertyContainer.IsContainerInitialized(base.SessionState))
			{
				ExchangePropertyContainer.SetServerSettings(base.SessionState, null);
			}
			base.SessionState.Variables[ExchangePropertyContainer.ADServerSettingsVarName] = null;
			TaskLogger.LogExit();
		}
	}
}
