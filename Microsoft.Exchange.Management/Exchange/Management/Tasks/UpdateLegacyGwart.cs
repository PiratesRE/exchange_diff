using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Update", "LegacyGwart", SupportsShouldProcess = true)]
	public sealed class UpdateLegacyGwart : SetupTaskBase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			((ITopologyConfigurationSession)this.configurationSession).UpdateGwartLastModified();
			TaskLogger.LogExit();
		}
	}
}
