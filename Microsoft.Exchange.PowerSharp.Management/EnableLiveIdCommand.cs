using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class EnableLiveIdCommand : SyntheticCommandWithPipelineInputNoOutput<uint>
	{
		private EnableLiveIdCommand() : base("Enable-LiveId")
		{
		}

		public EnableLiveIdCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual EnableLiveIdCommand SetParameters(EnableLiveIdCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual EnableLiveIdCommand SetParameters(EnableLiveIdCommand.PfxFileAndPasswordParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual EnableLiveIdCommand SetParameters(EnableLiveIdCommand.IssuedToParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual EnableLiveIdCommand SetParameters(EnableLiveIdCommand.SHA1ThumbprintParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual LiveIdInstanceType TargetInstance
			{
				set
				{
					base.PowerSharpParameters["TargetInstance"] = value;
				}
			}

			public virtual uint SiteId
			{
				set
				{
					base.PowerSharpParameters["SiteId"] = value;
				}
			}

			public virtual string SiteName
			{
				set
				{
					base.PowerSharpParameters["SiteName"] = value;
				}
			}

			public virtual uint AccrualSiteId
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteId"] = value;
				}
			}

			public virtual string AccrualSiteName
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteName"] = value;
				}
			}

			public virtual string InternalSiteName
			{
				set
				{
					base.PowerSharpParameters["InternalSiteName"] = value;
				}
			}

			public virtual string O365SiteName
			{
				set
				{
					base.PowerSharpParameters["O365SiteName"] = value;
				}
			}

			public virtual uint MsoSiteId
			{
				set
				{
					base.PowerSharpParameters["MsoSiteId"] = value;
				}
			}

			public virtual string MsoSiteName
			{
				set
				{
					base.PowerSharpParameters["MsoSiteName"] = value;
				}
			}

			public virtual TargetEnvironment TargetEnvironment
			{
				set
				{
					base.PowerSharpParameters["TargetEnvironment"] = value;
				}
			}

			public virtual string Proxy
			{
				set
				{
					base.PowerSharpParameters["Proxy"] = value;
				}
			}

			public virtual string MsoRpsNetworkProd
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkProd"] = value;
				}
			}

			public virtual string MsoRpsNetworkInt
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkInt"] = value;
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

		public class PfxFileAndPasswordParameters : ParametersBase
		{
			public virtual string Password
			{
				set
				{
					base.PowerSharpParameters["Password"] = value;
				}
			}

			public virtual string CertFile
			{
				set
				{
					base.PowerSharpParameters["CertFile"] = value;
				}
			}

			public virtual LiveIdInstanceType TargetInstance
			{
				set
				{
					base.PowerSharpParameters["TargetInstance"] = value;
				}
			}

			public virtual uint SiteId
			{
				set
				{
					base.PowerSharpParameters["SiteId"] = value;
				}
			}

			public virtual string SiteName
			{
				set
				{
					base.PowerSharpParameters["SiteName"] = value;
				}
			}

			public virtual uint AccrualSiteId
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteId"] = value;
				}
			}

			public virtual string AccrualSiteName
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteName"] = value;
				}
			}

			public virtual string InternalSiteName
			{
				set
				{
					base.PowerSharpParameters["InternalSiteName"] = value;
				}
			}

			public virtual string O365SiteName
			{
				set
				{
					base.PowerSharpParameters["O365SiteName"] = value;
				}
			}

			public virtual uint MsoSiteId
			{
				set
				{
					base.PowerSharpParameters["MsoSiteId"] = value;
				}
			}

			public virtual string MsoSiteName
			{
				set
				{
					base.PowerSharpParameters["MsoSiteName"] = value;
				}
			}

			public virtual TargetEnvironment TargetEnvironment
			{
				set
				{
					base.PowerSharpParameters["TargetEnvironment"] = value;
				}
			}

			public virtual string Proxy
			{
				set
				{
					base.PowerSharpParameters["Proxy"] = value;
				}
			}

			public virtual string MsoRpsNetworkProd
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkProd"] = value;
				}
			}

			public virtual string MsoRpsNetworkInt
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkInt"] = value;
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

		public class IssuedToParameters : ParametersBase
		{
			public virtual string IssuedTo
			{
				set
				{
					base.PowerSharpParameters["IssuedTo"] = value;
				}
			}

			public virtual LiveIdInstanceType TargetInstance
			{
				set
				{
					base.PowerSharpParameters["TargetInstance"] = value;
				}
			}

			public virtual uint SiteId
			{
				set
				{
					base.PowerSharpParameters["SiteId"] = value;
				}
			}

			public virtual string SiteName
			{
				set
				{
					base.PowerSharpParameters["SiteName"] = value;
				}
			}

			public virtual uint AccrualSiteId
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteId"] = value;
				}
			}

			public virtual string AccrualSiteName
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteName"] = value;
				}
			}

			public virtual string InternalSiteName
			{
				set
				{
					base.PowerSharpParameters["InternalSiteName"] = value;
				}
			}

			public virtual string O365SiteName
			{
				set
				{
					base.PowerSharpParameters["O365SiteName"] = value;
				}
			}

			public virtual uint MsoSiteId
			{
				set
				{
					base.PowerSharpParameters["MsoSiteId"] = value;
				}
			}

			public virtual string MsoSiteName
			{
				set
				{
					base.PowerSharpParameters["MsoSiteName"] = value;
				}
			}

			public virtual TargetEnvironment TargetEnvironment
			{
				set
				{
					base.PowerSharpParameters["TargetEnvironment"] = value;
				}
			}

			public virtual string Proxy
			{
				set
				{
					base.PowerSharpParameters["Proxy"] = value;
				}
			}

			public virtual string MsoRpsNetworkProd
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkProd"] = value;
				}
			}

			public virtual string MsoRpsNetworkInt
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkInt"] = value;
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

		public class SHA1ThumbprintParameters : ParametersBase
		{
			public virtual string SHA1Thumbprint
			{
				set
				{
					base.PowerSharpParameters["SHA1Thumbprint"] = value;
				}
			}

			public virtual string MsoSHA1Thumbprint
			{
				set
				{
					base.PowerSharpParameters["MsoSHA1Thumbprint"] = value;
				}
			}

			public virtual LiveIdInstanceType TargetInstance
			{
				set
				{
					base.PowerSharpParameters["TargetInstance"] = value;
				}
			}

			public virtual uint SiteId
			{
				set
				{
					base.PowerSharpParameters["SiteId"] = value;
				}
			}

			public virtual string SiteName
			{
				set
				{
					base.PowerSharpParameters["SiteName"] = value;
				}
			}

			public virtual uint AccrualSiteId
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteId"] = value;
				}
			}

			public virtual string AccrualSiteName
			{
				set
				{
					base.PowerSharpParameters["AccrualSiteName"] = value;
				}
			}

			public virtual string InternalSiteName
			{
				set
				{
					base.PowerSharpParameters["InternalSiteName"] = value;
				}
			}

			public virtual string O365SiteName
			{
				set
				{
					base.PowerSharpParameters["O365SiteName"] = value;
				}
			}

			public virtual uint MsoSiteId
			{
				set
				{
					base.PowerSharpParameters["MsoSiteId"] = value;
				}
			}

			public virtual string MsoSiteName
			{
				set
				{
					base.PowerSharpParameters["MsoSiteName"] = value;
				}
			}

			public virtual TargetEnvironment TargetEnvironment
			{
				set
				{
					base.PowerSharpParameters["TargetEnvironment"] = value;
				}
			}

			public virtual string Proxy
			{
				set
				{
					base.PowerSharpParameters["Proxy"] = value;
				}
			}

			public virtual string MsoRpsNetworkProd
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkProd"] = value;
				}
			}

			public virtual string MsoRpsNetworkInt
			{
				set
				{
					base.PowerSharpParameters["MsoRpsNetworkInt"] = value;
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
