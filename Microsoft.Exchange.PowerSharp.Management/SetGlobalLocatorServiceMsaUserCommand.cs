using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetGlobalLocatorServiceMsaUserCommand : SyntheticCommandWithPipelineInputNoOutput<NetID>
	{
		private SetGlobalLocatorServiceMsaUserCommand() : base("Set-GlobalLocatorServiceMsaUser")
		{
		}

		public SetGlobalLocatorServiceMsaUserCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetGlobalLocatorServiceMsaUserCommand SetParameters(SetGlobalLocatorServiceMsaUserCommand.MsaUserNetIDParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetGlobalLocatorServiceMsaUserCommand SetParameters(SetGlobalLocatorServiceMsaUserCommand.ExternalDirectoryOrganizationIdParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class MsaUserNetIDParameterSetParameters : ParametersBase
		{
			public virtual SmtpAddress MsaUserMemberName
			{
				set
				{
					base.PowerSharpParameters["MsaUserMemberName"] = value;
				}
			}

			public virtual NetID MsaUserNetId
			{
				set
				{
					base.PowerSharpParameters["MsaUserNetId"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class ExternalDirectoryOrganizationIdParameterSetParameters : ParametersBase
		{
			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
