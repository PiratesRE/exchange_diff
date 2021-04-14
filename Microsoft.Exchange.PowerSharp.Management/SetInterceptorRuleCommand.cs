using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Agent.InterceptorAgent;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetInterceptorRuleCommand : SyntheticCommandWithPipelineInputNoOutput<InterceptorAgentRule>
	{
		private SetInterceptorRuleCommand() : base("Set-InterceptorRule")
		{
		}

		public SetInterceptorRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetInterceptorRuleCommand SetParameters(SetInterceptorRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetInterceptorRuleCommand SetParameters(SetInterceptorRuleCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual InterceptorAgentRuleBehavior Action
			{
				set
				{
					base.PowerSharpParameters["Action"] = value;
				}
			}

			public virtual InterceptorAgentEvent Event
			{
				set
				{
					base.PowerSharpParameters["Event"] = value;
				}
			}

			public virtual string Condition
			{
				set
				{
					base.PowerSharpParameters["Condition"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual MultiValuedProperty<DatabaseAvailabilityGroupIdParameter> Dag
			{
				set
				{
					base.PowerSharpParameters["Dag"] = value;
				}
			}

			public virtual MultiValuedProperty<AdSiteIdParameter> Site
			{
				set
				{
					base.PowerSharpParameters["Site"] = value;
				}
			}

			public virtual EnhancedTimeSpan TimeInterval
			{
				set
				{
					base.PowerSharpParameters["TimeInterval"] = value;
				}
			}

			public virtual string CustomResponseString
			{
				set
				{
					base.PowerSharpParameters["CustomResponseString"] = value;
				}
			}

			public virtual string CustomResponseCode
			{
				set
				{
					base.PowerSharpParameters["CustomResponseCode"] = value;
				}
			}

			public virtual DateTime ExpireTime
			{
				set
				{
					base.PowerSharpParameters["ExpireTime"] = value;
				}
			}

			public virtual string Path
			{
				set
				{
					base.PowerSharpParameters["Path"] = value;
				}
			}

			public virtual SourceType Source
			{
				set
				{
					base.PowerSharpParameters["Source"] = value;
				}
			}

			public virtual string CreatedBy
			{
				set
				{
					base.PowerSharpParameters["CreatedBy"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new InterceptorRuleIdParameter(value) : null);
				}
			}

			public virtual InterceptorAgentRuleBehavior Action
			{
				set
				{
					base.PowerSharpParameters["Action"] = value;
				}
			}

			public virtual InterceptorAgentEvent Event
			{
				set
				{
					base.PowerSharpParameters["Event"] = value;
				}
			}

			public virtual string Condition
			{
				set
				{
					base.PowerSharpParameters["Condition"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual MultiValuedProperty<DatabaseAvailabilityGroupIdParameter> Dag
			{
				set
				{
					base.PowerSharpParameters["Dag"] = value;
				}
			}

			public virtual MultiValuedProperty<AdSiteIdParameter> Site
			{
				set
				{
					base.PowerSharpParameters["Site"] = value;
				}
			}

			public virtual EnhancedTimeSpan TimeInterval
			{
				set
				{
					base.PowerSharpParameters["TimeInterval"] = value;
				}
			}

			public virtual string CustomResponseString
			{
				set
				{
					base.PowerSharpParameters["CustomResponseString"] = value;
				}
			}

			public virtual string CustomResponseCode
			{
				set
				{
					base.PowerSharpParameters["CustomResponseCode"] = value;
				}
			}

			public virtual DateTime ExpireTime
			{
				set
				{
					base.PowerSharpParameters["ExpireTime"] = value;
				}
			}

			public virtual string Path
			{
				set
				{
					base.PowerSharpParameters["Path"] = value;
				}
			}

			public virtual SourceType Source
			{
				set
				{
					base.PowerSharpParameters["Source"] = value;
				}
			}

			public virtual string CreatedBy
			{
				set
				{
					base.PowerSharpParameters["CreatedBy"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
