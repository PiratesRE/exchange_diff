using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallDatacenterRoleBaseDataHandler : InstallRoleBaseDataHandler
	{
		public InstallDatacenterRoleBaseDataHandler(ISetupContext context, string installableUnitName, string commandText, MonadConnection connection) : base(context, installableUnitName, commandText, connection)
		{
		}
	}
}
