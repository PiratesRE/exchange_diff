﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "SyncDaemonArbitrationConfig")]
	public sealed class GetSyncDaemonArbitrationConfig : GetSingletonSystemConfigurationObjectTask<SyncDaemonArbitrationConfig>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
