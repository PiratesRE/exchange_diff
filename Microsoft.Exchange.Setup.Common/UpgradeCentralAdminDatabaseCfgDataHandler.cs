﻿using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeCentralAdminDatabaseCfgDataHandler : UpgradeCfgDataHandler
	{
		public UpgradeCentralAdminDatabaseCfgDataHandler(ISetupContext context, Role roleInfo, MonadConnection connection) : base(context, roleInfo, connection)
		{
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}
	}
}
