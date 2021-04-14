using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class HostedContentFilterRuleSchema : HygieneFilterRuleSchema
	{
		public static readonly ADPropertyDefinition HostedContentFilterPolicy = new ADPropertyDefinition("HostedContentFilterPolicy", ExchangeObjectVersion.Exchange2012, typeof(HostedContentFilterPolicyIdParameter), "hostedContentFilterPolicy", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
