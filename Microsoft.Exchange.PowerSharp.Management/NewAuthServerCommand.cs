using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewAuthServerCommand : SyntheticCommandWithPipelineInput<AuthServer, AuthServer>
	{
		private NewAuthServerCommand() : base("New-AuthServer")
		{
		}

		public NewAuthServerCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewAuthServerCommand SetParameters(NewAuthServerCommand.AppSecretParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAuthServerCommand SetParameters(NewAuthServerCommand.NativeClientAuthServerParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAuthServerCommand SetParameters(NewAuthServerCommand.AuthMetadataUrlParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewAuthServerCommand SetParameters(NewAuthServerCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class AppSecretParameterSetParameters : ParametersBase
		{
			public virtual string AppSecret
			{
				set
				{
					base.PowerSharpParameters["AppSecret"] = value;
				}
			}

			public virtual string IssuerIdentifier
			{
				set
				{
					base.PowerSharpParameters["IssuerIdentifier"] = value;
				}
			}

			public virtual string TokenIssuingEndpoint
			{
				set
				{
					base.PowerSharpParameters["TokenIssuingEndpoint"] = value;
				}
			}

			public virtual string AuthorizationEndpoint
			{
				set
				{
					base.PowerSharpParameters["AuthorizationEndpoint"] = value;
				}
			}

			public virtual string ApplicationIdentifier
			{
				set
				{
					base.PowerSharpParameters["ApplicationIdentifier"] = value;
				}
			}

			public virtual AuthServerType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
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

		public class NativeClientAuthServerParameterSetParameters : ParametersBase
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

			public virtual AuthServerType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
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

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
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
