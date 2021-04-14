using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewUMDialPlanCommand : SyntheticCommandWithPipelineInput<UMDialPlan, UMDialPlan>
	{
		private NewUMDialPlanCommand() : base("New-UMDialPlan")
		{
		}

		public NewUMDialPlanCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewUMDialPlanCommand SetParameters(NewUMDialPlanCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual int NumberOfDigitsInExtension
			{
				set
				{
					base.PowerSharpParameters["NumberOfDigitsInExtension"] = value;
				}
			}

			public virtual UMUriType URIType
			{
				set
				{
					base.PowerSharpParameters["URIType"] = value;
				}
			}

			public virtual UMSubscriberType SubscriberType
			{
				set
				{
					base.PowerSharpParameters["SubscriberType"] = value;
				}
			}

			public virtual UMVoIPSecurityType VoIPSecurity
			{
				set
				{
					base.PowerSharpParameters["VoIPSecurity"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AccessTelephoneNumbers
			{
				set
				{
					base.PowerSharpParameters["AccessTelephoneNumbers"] = value;
				}
			}

			public virtual bool FaxEnabled
			{
				set
				{
					base.PowerSharpParameters["FaxEnabled"] = value;
				}
			}

			public virtual bool SipResourceIdentifierRequired
			{
				set
				{
					base.PowerSharpParameters["SipResourceIdentifierRequired"] = value;
				}
			}

			public virtual string DefaultOutboundCallingLineId
			{
				set
				{
					base.PowerSharpParameters["DefaultOutboundCallingLineId"] = value;
				}
			}

			public virtual bool GenerateUMMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["GenerateUMMailboxPolicy"] = value;
				}
			}

			public virtual string CountryOrRegionCode
			{
				set
				{
					base.PowerSharpParameters["CountryOrRegionCode"] = value;
				}
			}

			public virtual UMGlobalCallRoutingScheme GlobalCallRoutingScheme
			{
				set
				{
					base.PowerSharpParameters["GlobalCallRoutingScheme"] = value;
				}
			}

			public virtual UMLanguage DefaultLanguage
			{
				set
				{
					base.PowerSharpParameters["DefaultLanguage"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
