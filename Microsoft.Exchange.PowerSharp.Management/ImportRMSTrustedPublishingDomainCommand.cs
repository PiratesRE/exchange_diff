using System;
using System.Collections;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class ImportRMSTrustedPublishingDomainCommand : SyntheticCommandWithPipelineInput<RMSTrustedPublishingDomain, RMSTrustedPublishingDomain>
	{
		private ImportRMSTrustedPublishingDomainCommand() : base("Import-RMSTrustedPublishingDomain")
		{
		}

		public ImportRMSTrustedPublishingDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual ImportRMSTrustedPublishingDomainCommand SetParameters(ImportRMSTrustedPublishingDomainCommand.RefreshTemplatesParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportRMSTrustedPublishingDomainCommand SetParameters(ImportRMSTrustedPublishingDomainCommand.ImportFromFileParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportRMSTrustedPublishingDomainCommand SetParameters(ImportRMSTrustedPublishingDomainCommand.IntranetLicensingUrlParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportRMSTrustedPublishingDomainCommand SetParameters(ImportRMSTrustedPublishingDomainCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportRMSTrustedPublishingDomainCommand SetParameters(ImportRMSTrustedPublishingDomainCommand.RMSOnlineParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual ImportRMSTrustedPublishingDomainCommand SetParameters(ImportRMSTrustedPublishingDomainCommand.RMSOnline2Parameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class RefreshTemplatesParameters : ParametersBase
		{
			public virtual SecureString Password
			{
				set
				{
					base.PowerSharpParameters["Password"] = value;
				}
			}

			public virtual byte FileData
			{
				set
				{
					base.PowerSharpParameters["FileData"] = value;
				}
			}

			public virtual SwitchParameter RefreshTemplates
			{
				set
				{
					base.PowerSharpParameters["RefreshTemplates"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
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

		public class ImportFromFileParameters : ParametersBase
		{
			public virtual SecureString Password
			{
				set
				{
					base.PowerSharpParameters["Password"] = value;
				}
			}

			public virtual byte FileData
			{
				set
				{
					base.PowerSharpParameters["FileData"] = value;
				}
			}

			public virtual Uri IntranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetLicensingUrl"] = value;
				}
			}

			public virtual Uri ExtranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetLicensingUrl"] = value;
				}
			}

			public virtual Uri IntranetCertificationUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetCertificationUrl"] = value;
				}
			}

			public virtual Uri ExtranetCertificationUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetCertificationUrl"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
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

		public class IntranetLicensingUrlParameters : ParametersBase
		{
			public virtual SecureString Password
			{
				set
				{
					base.PowerSharpParameters["Password"] = value;
				}
			}

			public virtual byte FileData
			{
				set
				{
					base.PowerSharpParameters["FileData"] = value;
				}
			}

			public virtual Uri IntranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetLicensingUrl"] = value;
				}
			}

			public virtual Uri ExtranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetLicensingUrl"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
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

		public class RMSOnlineParameters : ParametersBase
		{
			public virtual SwitchParameter RefreshTemplates
			{
				set
				{
					base.PowerSharpParameters["RefreshTemplates"] = value;
				}
			}

			public virtual SwitchParameter RMSOnline
			{
				set
				{
					base.PowerSharpParameters["RMSOnline"] = value;
				}
			}

			public virtual Guid RMSOnlineOrgOverride
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineOrgOverride"] = value;
				}
			}

			public virtual string RMSOnlineAuthCertSubjectNameOverride
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineAuthCertSubjectNameOverride"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
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

		public class RMSOnline2Parameters : ParametersBase
		{
			public virtual byte RMSOnlineConfig
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineConfig"] = value;
				}
			}

			public virtual Hashtable RMSOnlineKeys
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineKeys"] = value;
				}
			}

			public virtual Hashtable RMSOnlineAuthorTest
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineAuthorTest"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
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
