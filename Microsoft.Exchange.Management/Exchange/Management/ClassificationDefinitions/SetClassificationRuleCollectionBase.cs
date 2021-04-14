using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	public abstract class SetClassificationRuleCollectionBase : SetSystemConfigurationObjectTask<ClassificationRuleCollectionIdParameter, TransportRule>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = false, ValueFromPipeline = false)]
		public override ClassificationRuleCollectionIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}
	}
}
