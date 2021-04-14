using System;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	public interface ITenantColumn
	{
		Guid TenantGuid { get; set; }
	}
}
