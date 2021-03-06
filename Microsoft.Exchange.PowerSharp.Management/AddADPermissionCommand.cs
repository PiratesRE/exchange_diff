using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class AddADPermissionCommand : SyntheticCommandWithPipelineInput<ADAcePresentationObject, ADAcePresentationObject>
	{
		private AddADPermissionCommand() : base("Add-ADPermission")
		{
		}

		public AddADPermissionCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual AddADPermissionCommand SetParameters(AddADPermissionCommand.OwnerParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddADPermissionCommand SetParameters(AddADPermissionCommand.AccessRightsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddADPermissionCommand SetParameters(AddADPermissionCommand.InstanceParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddADPermissionCommand SetParameters(AddADPermissionCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class OwnerParameters : ParametersBase
		{
			public virtual string Owner
			{
				set
				{
					base.PowerSharpParameters["Owner"] = ((value != null) ? new SecurityPrincipalIdParameter(value) : null);
				}
			}

			public virtual ADRawEntryIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
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

		public class AccessRightsParameters : ParametersBase
		{
			public virtual ADRawEntryIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual ActiveDirectoryRights AccessRights
			{
				set
				{
					base.PowerSharpParameters["AccessRights"] = value;
				}
			}

			public virtual ExtendedRightIdParameter ExtendedRights
			{
				set
				{
					base.PowerSharpParameters["ExtendedRights"] = value;
				}
			}

			public virtual ADSchemaObjectIdParameter ChildObjectTypes
			{
				set
				{
					base.PowerSharpParameters["ChildObjectTypes"] = value;
				}
			}

			public virtual ADSchemaObjectIdParameter InheritedObjectType
			{
				set
				{
					base.PowerSharpParameters["InheritedObjectType"] = value;
				}
			}

			public virtual ADSchemaObjectIdParameter Properties
			{
				set
				{
					base.PowerSharpParameters["Properties"] = value;
				}
			}

			public virtual SwitchParameter Deny
			{
				set
				{
					base.PowerSharpParameters["Deny"] = value;
				}
			}

			public virtual ActiveDirectorySecurityInheritance InheritanceType
			{
				set
				{
					base.PowerSharpParameters["InheritanceType"] = value;
				}
			}

			public virtual string User
			{
				set
				{
					base.PowerSharpParameters["User"] = ((value != null) ? new SecurityPrincipalIdParameter(value) : null);
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

		public class InstanceParameters : ParametersBase
		{
			public virtual ADRawEntryIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual ADAcePresentationObject Instance
			{
				set
				{
					base.PowerSharpParameters["Instance"] = value;
				}
			}

			public virtual ActiveDirectoryRights AccessRights
			{
				set
				{
					base.PowerSharpParameters["AccessRights"] = value;
				}
			}

			public virtual ExtendedRightIdParameter ExtendedRights
			{
				set
				{
					base.PowerSharpParameters["ExtendedRights"] = value;
				}
			}

			public virtual ADSchemaObjectIdParameter ChildObjectTypes
			{
				set
				{
					base.PowerSharpParameters["ChildObjectTypes"] = value;
				}
			}

			public virtual ADSchemaObjectIdParameter InheritedObjectType
			{
				set
				{
					base.PowerSharpParameters["InheritedObjectType"] = value;
				}
			}

			public virtual ADSchemaObjectIdParameter Properties
			{
				set
				{
					base.PowerSharpParameters["Properties"] = value;
				}
			}

			public virtual SwitchParameter Deny
			{
				set
				{
					base.PowerSharpParameters["Deny"] = value;
				}
			}

			public virtual ActiveDirectorySecurityInheritance InheritanceType
			{
				set
				{
					base.PowerSharpParameters["InheritanceType"] = value;
				}
			}

			public virtual string User
			{
				set
				{
					base.PowerSharpParameters["User"] = ((value != null) ? new SecurityPrincipalIdParameter(value) : null);
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

		public class DefaultParameters : ParametersBase
		{
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
