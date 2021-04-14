using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetManagementRoleAssignmentCommand : SyntheticCommandWithPipelineInput<ExchangeRoleAssignment, ExchangeRoleAssignment>
	{
		private GetManagementRoleAssignmentCommand() : base("Get-ManagementRoleAssignment")
		{
		}

		public GetManagementRoleAssignmentCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetManagementRoleAssignmentCommand SetParameters(GetManagementRoleAssignmentCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetManagementRoleAssignmentCommand SetParameters(GetManagementRoleAssignmentCommand.RoleAssigneeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetManagementRoleAssignmentCommand SetParameters(GetManagementRoleAssignmentCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual RoleAssigneeType RoleAssigneeType
			{
				set
				{
					base.PowerSharpParameters["RoleAssigneeType"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool Delegating
			{
				set
				{
					base.PowerSharpParameters["Delegating"] = value;
				}
			}

			public virtual bool Exclusive
			{
				set
				{
					base.PowerSharpParameters["Exclusive"] = value;
				}
			}

			public virtual RecipientWriteScopeType RecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["RecipientWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter CustomRecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["CustomRecipientWriteScope"] = value;
				}
			}

			public virtual string RecipientOrganizationalUnitScope
			{
				set
				{
					base.PowerSharpParameters["RecipientOrganizationalUnitScope"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual ConfigWriteScopeType ConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["ConfigWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter CustomConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["CustomConfigWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter ExclusiveRecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["ExclusiveRecipientWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter ExclusiveConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["ExclusiveConfigWriteScope"] = value;
				}
			}

			public virtual SwitchParameter GetEffectiveUsers
			{
				set
				{
					base.PowerSharpParameters["GetEffectiveUsers"] = value;
				}
			}

			public virtual string WritableRecipient
			{
				set
				{
					base.PowerSharpParameters["WritableRecipient"] = ((value != null) ? new GeneralRecipientIdParameter(value) : null);
				}
			}

			public virtual ServerIdParameter WritableServer
			{
				set
				{
					base.PowerSharpParameters["WritableServer"] = value;
				}
			}

			public virtual DatabaseIdParameter WritableDatabase
			{
				set
				{
					base.PowerSharpParameters["WritableDatabase"] = value;
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

		public class RoleAssigneeParameters : ParametersBase
		{
			public virtual string Role
			{
				set
				{
					base.PowerSharpParameters["Role"] = ((value != null) ? new RoleIdParameter(value) : null);
				}
			}

			public virtual string RoleAssignee
			{
				set
				{
					base.PowerSharpParameters["RoleAssignee"] = ((value != null) ? new RoleAssigneeIdParameter(value) : null);
				}
			}

			public virtual AssignmentMethod AssignmentMethod
			{
				set
				{
					base.PowerSharpParameters["AssignmentMethod"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual RoleAssigneeType RoleAssigneeType
			{
				set
				{
					base.PowerSharpParameters["RoleAssigneeType"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool Delegating
			{
				set
				{
					base.PowerSharpParameters["Delegating"] = value;
				}
			}

			public virtual bool Exclusive
			{
				set
				{
					base.PowerSharpParameters["Exclusive"] = value;
				}
			}

			public virtual RecipientWriteScopeType RecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["RecipientWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter CustomRecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["CustomRecipientWriteScope"] = value;
				}
			}

			public virtual string RecipientOrganizationalUnitScope
			{
				set
				{
					base.PowerSharpParameters["RecipientOrganizationalUnitScope"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual ConfigWriteScopeType ConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["ConfigWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter CustomConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["CustomConfigWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter ExclusiveRecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["ExclusiveRecipientWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter ExclusiveConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["ExclusiveConfigWriteScope"] = value;
				}
			}

			public virtual SwitchParameter GetEffectiveUsers
			{
				set
				{
					base.PowerSharpParameters["GetEffectiveUsers"] = value;
				}
			}

			public virtual string WritableRecipient
			{
				set
				{
					base.PowerSharpParameters["WritableRecipient"] = ((value != null) ? new GeneralRecipientIdParameter(value) : null);
				}
			}

			public virtual ServerIdParameter WritableServer
			{
				set
				{
					base.PowerSharpParameters["WritableServer"] = value;
				}
			}

			public virtual DatabaseIdParameter WritableDatabase
			{
				set
				{
					base.PowerSharpParameters["WritableDatabase"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RoleAssignmentIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual RoleAssigneeType RoleAssigneeType
			{
				set
				{
					base.PowerSharpParameters["RoleAssigneeType"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool Delegating
			{
				set
				{
					base.PowerSharpParameters["Delegating"] = value;
				}
			}

			public virtual bool Exclusive
			{
				set
				{
					base.PowerSharpParameters["Exclusive"] = value;
				}
			}

			public virtual RecipientWriteScopeType RecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["RecipientWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter CustomRecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["CustomRecipientWriteScope"] = value;
				}
			}

			public virtual string RecipientOrganizationalUnitScope
			{
				set
				{
					base.PowerSharpParameters["RecipientOrganizationalUnitScope"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual ConfigWriteScopeType ConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["ConfigWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter CustomConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["CustomConfigWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter ExclusiveRecipientWriteScope
			{
				set
				{
					base.PowerSharpParameters["ExclusiveRecipientWriteScope"] = value;
				}
			}

			public virtual ManagementScopeIdParameter ExclusiveConfigWriteScope
			{
				set
				{
					base.PowerSharpParameters["ExclusiveConfigWriteScope"] = value;
				}
			}

			public virtual SwitchParameter GetEffectiveUsers
			{
				set
				{
					base.PowerSharpParameters["GetEffectiveUsers"] = value;
				}
			}

			public virtual string WritableRecipient
			{
				set
				{
					base.PowerSharpParameters["WritableRecipient"] = ((value != null) ? new GeneralRecipientIdParameter(value) : null);
				}
			}

			public virtual ServerIdParameter WritableServer
			{
				set
				{
					base.PowerSharpParameters["WritableServer"] = value;
				}
			}

			public virtual DatabaseIdParameter WritableDatabase
			{
				set
				{
					base.PowerSharpParameters["WritableDatabase"] = value;
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
