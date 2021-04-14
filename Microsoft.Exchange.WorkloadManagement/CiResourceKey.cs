using System;
using Microsoft.Exchange.Data.Storage.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal abstract class CiResourceKey : MdbResourceKey
	{
		protected CiResourceKey(ResourceMetricType metric, Guid mdbGuid) : base(metric, mdbGuid)
		{
		}
	}
}
