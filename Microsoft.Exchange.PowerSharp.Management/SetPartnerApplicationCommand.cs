using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetPartnerApplicationCommand : SyntheticCommandWithPipelineInputNoOutput<PartnerApplication>
	{
		private SetPartnerApplicationCommand() : base("Set-PartnerApplication")
		{
		}

		public SetPartnerApplicationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetPartnerApplicationCommand SetParameters(SetPartnerApplicationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPartnerApplicationCommand SetParameters(SetPartnerApplicationCommand.ACSTrustApplicationParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPartnerApplicationCommand SetParameters(SetPartnerApplicationCommand.AuthMetadataUrlParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPartnerApplicationCommand SetParameters(SetPartnerApplicationCommand.RefreshAuthMetadataParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PartnerApplicationIdParameter(value) : null);
				}
			}

			public virtual string LinkedAccount
			{
				set
				{
					base.PowerSharpParameters["LinkedAccount"] = ((value != null) ? new UserIdParameter(value) : null);
				}
			}

			public virtual string IssuerIdentifier
			{
				set
				{
					base.PowerSharpParameters["IssuerIdentifier"] = value;
				}
			}

			public virtual string AppOnlyPermissions
			{
				set
				{
					base.PowerSharpParameters["AppOnlyPermissions"] = value;
				}
			}

			public virtual string ActAsPermissions
			{
				set
				{
					base.PowerSharpParameters["ActAsPermissions"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool AcceptSecurityIdentifierInformation
			{
				set
				{
					base.PowerSharpParameters["AcceptSecurityIdentifierInformation"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class ACSTrustApplicationParameterSetParameters : ParametersBase
		{
			public virtual string ApplicationIdentifier
			{
				set
				{
					base.PowerSharpParameters["ApplicationIdentifier"] = value;
				}
			}

			public virtual string Realm
			{
				set
				{
					base.PowerSharpParameters["Realm"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PartnerApplicationIdParameter(value) : null);
				}
			}

			public virtual string LinkedAccount
			{
				set
				{
					base.PowerSharpParameters["LinkedAccount"] = ((value != null) ? new UserIdParameter(value) : null);
				}
			}

			public virtual string IssuerIdentifier
			{
				set
				{
					base.PowerSharpParameters["IssuerIdentifier"] = value;
				}
			}

			public virtual string AppOnlyPermissions
			{
				set
				{
					base.PowerSharpParameters["AppOnlyPermissions"] = value;
				}
			}

			public virtual string ActAsPermissions
			{
				set
				{
					base.PowerSharpParameters["ActAsPermissions"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool AcceptSecurityIdentifierInformation
			{
				set
				{
					base.PowerSharpParameters["AcceptSecurityIdentifierInformation"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class AuthMetadataUrlParameterSetParameters : ParametersBase
		{
			public virtual string AuthMetadataUrl
			{
				set
				{
					base.PowerSharpParameters["AuthMetadataUrl"] = value;
				}
			}

			public virtual SwitchParameter TrustAnySSLCertificate
			{
				set
				{
					base.PowerSharpParameters["TrustAnySSLCertificate"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PartnerApplicationIdParameter(value) : null);
				}
			}

			public virtual string LinkedAccount
			{
				set
				{
					base.PowerSharpParameters["LinkedAccount"] = ((value != null) ? new UserIdParameter(value) : null);
				}
			}

			public virtual string IssuerIdentifier
			{
				set
				{
					base.PowerSharpParameters["IssuerIdentifier"] = value;
				}
			}

			public virtual string AppOnlyPermissions
			{
				set
				{
					base.PowerSharpParameters["AppOnlyPermissions"] = value;
				}
			}

			public virtual string ActAsPermissions
			{
				set
				{
					base.PowerSharpParameters["ActAsPermissions"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool AcceptSecurityIdentifierInformation
			{
				set
				{
					base.PowerSharpParameters["AcceptSecurityIdentifierInformation"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class RefreshAuthMetadataParameterSetParameters : ParametersBase
		{
			public virtual SwitchParameter RefreshAuthMetadata
			{
				set
				{
					base.PowerSharpParameters["RefreshAuthMetadata"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PartnerApplicationIdParameter(value) : null);
				}
			}

			public virtual string LinkedAccount
			{
				set
				{
					base.PowerSharpParameters["LinkedAccount"] = ((value != null) ? new UserIdParameter(value) : null);
				}
			}

			public virtual string IssuerIdentifier
			{
				set
				{
					base.PowerSharpParameters["IssuerIdentifier"] = value;
				}
			}

			public virtual string AppOnlyPermissions
			{
				set
				{
					base.PowerSharpParameters["AppOnlyPermissions"] = value;
				}
			}

			public virtual string ActAsPermissions
			{
				set
				{
					base.PowerSharpParameters["ActAsPermissions"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool AcceptSecurityIdentifierInformation
			{
				set
				{
					base.PowerSharpParameters["AcceptSecurityIdentifierInformation"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
