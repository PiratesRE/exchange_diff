using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[OutputType(new Type[]
	{
		typeof(DynamicDistributionGroup)
	})]
	[Cmdlet("Get", "DynamicDistributionGroup", DefaultParameterSetName = "Identity")]
	public sealed class GetDynamicDistributionGroup : GetDynamicDistributionGroupBase
	{
	}
}
