using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetSecurityPrincipalCommand : SyntheticCommandWithPipelineInput<ExtendedSecurityPrincipal, ExtendedSecurityPrincipal>
	{
		private GetSecurityPrincipalCommand() : base("Get-SecurityPrincipal")
		{
		}

		public GetSecurityPrincipalCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetSecurityPrincipalCommand SetParameters(GetSecurityPrincipalCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetSecurityPrincipalCommand SetParameters(GetSecurityPrincipalCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string OrganizationalUnit
			{
				set
				{
					base.PowerSharpParameters["OrganizationalUnit"] = ((value != null) ? new ExtendedOrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SmtpDomain IncludeDomainLocalFrom
			{
				set
				{
					base.PowerSharpParameters["IncludeDomainLocalFrom"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual MultiValuedProperty<SecurityPrincipalType> Types
			{
				set
				{
					base.PowerSharpParameters["Types"] = value;
				}
			}

			public virtual SwitchParameter RoleGroupAssignable
			{
				set
				{
					base.PowerSharpParameters["RoleGroupAssignable"] = value;
				}
			}

			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExtendedSecurityPrincipalIdParameter(value) : null);
				}
			}

			public virtual string OrganizationalUnit
			{
				set
				{
					base.PowerSharpParameters["OrganizationalUnit"] = ((value != null) ? new ExtendedOrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SmtpDomain IncludeDomainLocalFrom
			{
				set
				{
					base.PowerSharpParameters["IncludeDomainLocalFrom"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual MultiValuedProperty<SecurityPrincipalType> Types
			{
				set
				{
					base.PowerSharpParameters["Types"] = value;
				}
			}

			public virtual SwitchParameter RoleGroupAssignable
			{
				set
				{
					base.PowerSharpParameters["RoleGroupAssignable"] = value;
				}
			}

			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
		}
	}
}
