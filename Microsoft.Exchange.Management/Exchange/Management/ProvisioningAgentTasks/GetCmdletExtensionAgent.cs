using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningAgentTasks
{
	[Cmdlet("Get", "CmdletExtensionAgent", DefaultParameterSetName = "Identity")]
	public sealed class GetCmdletExtensionAgent : GetSystemConfigurationObjectTask<CmdletExtensionAgentIdParameter, CmdletExtensionAgent>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filters")]
		public string Assembly
		{
			get
			{
				return (string)base.Fields["Assembly"];
			}
			set
			{
				base.Fields["Assembly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filters")]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return this.internalFilter ?? base.InternalFilter;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			CmdletExtensionAgentsGlobalConfig cmdletExtensionAgentsGlobalConfig = new CmdletExtensionAgentsGlobalConfig((ITopologyConfigurationSession)base.DataSession);
			foreach (LocalizedString text in cmdletExtensionAgentsGlobalConfig.ConfigurationIssues)
			{
				this.WriteWarning(text);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (this.Assembly != null)
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, CmdletExtensionAgentSchema.Assembly, this.Assembly);
				if (this.InternalFilter == null)
				{
					this.internalFilter = queryFilter;
				}
				else
				{
					this.internalFilter = new AndFilter(new QueryFilter[]
					{
						this.InternalFilter,
						queryFilter
					});
				}
			}
			if (base.Fields["Enabled"] != null)
			{
				QueryFilter queryFilter2 = new BitMaskAndFilter(CmdletExtensionAgentSchema.CmdletExtensionFlags, 1UL);
				if (!this.Enabled)
				{
					queryFilter2 = new NotFilter(queryFilter2);
				}
				if (this.InternalFilter == null)
				{
					this.internalFilter = queryFilter2;
				}
				else
				{
					this.internalFilter = new AndFilter(new QueryFilter[]
					{
						this.InternalFilter,
						queryFilter2
					});
				}
			}
			base.InternalValidate();
		}

		private QueryFilter internalFilter;
	}
}
