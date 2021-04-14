using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class RemoveVirtualDirectory<T> : RemoveSystemConfigurationObjectTask<VirtualDirectoryIdParameter, T> where T : ADVirtualDirectory, new()
	{
	}
}
