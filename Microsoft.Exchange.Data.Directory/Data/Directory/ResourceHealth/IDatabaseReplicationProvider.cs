using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal interface IDatabaseReplicationProvider : IResourceLoadMonitor
	{
		void Update(uint databaseReplicationHealth);
	}
}
