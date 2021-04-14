﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("uninstall", "AddressRewriteConfigContainer")]
	public sealed class UninstallAddressRewriteConfigContainerTask : RemoveSystemConfigurationObjectTask<ContainerIdParameter, AddressRewriteConfigContainer>
	{
	}
}
