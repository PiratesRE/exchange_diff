using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class AddressListPagingTaskBase : SystemConfigurationObjectActionTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Identity != null)
			{
				base.InternalValidate();
			}
			TaskLogger.LogExit();
		}
	}
}
