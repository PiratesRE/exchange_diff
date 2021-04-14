using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Office.ComplianceJob.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetComplianceSearchCommand : SyntheticCommandWithPipelineInputNoOutput<ComplianceSearch>
	{
		private SetComplianceSearchCommand() : base("Set-ComplianceSearch")
		{
		}

		public SetComplianceSearchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetComplianceSearchCommand SetParameters(SetComplianceSearchCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetComplianceSearchCommand SetParameters(SetComplianceSearchCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string KeywordQuery
			{
				set
				{
					base.PowerSharpParameters["KeywordQuery"] = value;
				}
			}

			public virtual DateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual DateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual CultureInfo Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual bool IncludeUnindexedItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnindexedItems"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual string ExchangeBinding
			{
				set
				{
					base.PowerSharpParameters["ExchangeBinding"] = value;
				}
			}

			public virtual string PublicFolderBinding
			{
				set
				{
					base.PowerSharpParameters["PublicFolderBinding"] = value;
				}
			}

			public virtual string SharePointBinding
			{
				set
				{
					base.PowerSharpParameters["SharePointBinding"] = value;
				}
			}

			public virtual string AddExchangeBinding
			{
				set
				{
					base.PowerSharpParameters["AddExchangeBinding"] = value;
				}
			}

			public virtual string AddPublicFolderBinding
			{
				set
				{
					base.PowerSharpParameters["AddPublicFolderBinding"] = value;
				}
			}

			public virtual string AddSharePointBinding
			{
				set
				{
					base.PowerSharpParameters["AddSharePointBinding"] = value;
				}
			}

			public virtual string RemoveExchangeBinding
			{
				set
				{
					base.PowerSharpParameters["RemoveExchangeBinding"] = value;
				}
			}

			public virtual string RemovePublicFolderBinding
			{
				set
				{
					base.PowerSharpParameters["RemovePublicFolderBinding"] = value;
				}
			}

			public virtual string RemoveSharePointBinding
			{
				set
				{
					base.PowerSharpParameters["RemoveSharePointBinding"] = value;
				}
			}

			public virtual bool AllPublicFolderBindings
			{
				set
				{
					base.PowerSharpParameters["AllPublicFolderBindings"] = value;
				}
			}

			public virtual bool AllExchangeBindings
			{
				set
				{
					base.PowerSharpParameters["AllExchangeBindings"] = value;
				}
			}

			public virtual bool AllSharePointBindings
			{
				set
				{
					base.PowerSharpParameters["AllSharePointBindings"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ComplianceJobIdParameter(value) : null);
				}
			}

			public virtual string KeywordQuery
			{
				set
				{
					base.PowerSharpParameters["KeywordQuery"] = value;
				}
			}

			public virtual DateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual DateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual CultureInfo Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual bool IncludeUnindexedItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnindexedItems"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
				}
			}

			public virtual string ExchangeBinding
			{
				set
				{
					base.PowerSharpParameters["ExchangeBinding"] = value;
				}
			}

			public virtual string PublicFolderBinding
			{
				set
				{
					base.PowerSharpParameters["PublicFolderBinding"] = value;
				}
			}

			public virtual string SharePointBinding
			{
				set
				{
					base.PowerSharpParameters["SharePointBinding"] = value;
				}
			}

			public virtual string AddExchangeBinding
			{
				set
				{
					base.PowerSharpParameters["AddExchangeBinding"] = value;
				}
			}

			public virtual string AddPublicFolderBinding
			{
				set
				{
					base.PowerSharpParameters["AddPublicFolderBinding"] = value;
				}
			}

			public virtual string AddSharePointBinding
			{
				set
				{
					base.PowerSharpParameters["AddSharePointBinding"] = value;
				}
			}

			public virtual string RemoveExchangeBinding
			{
				set
				{
					base.PowerSharpParameters["RemoveExchangeBinding"] = value;
				}
			}

			public virtual string RemovePublicFolderBinding
			{
				set
				{
					base.PowerSharpParameters["RemovePublicFolderBinding"] = value;
				}
			}

			public virtual string RemoveSharePointBinding
			{
				set
				{
					base.PowerSharpParameters["RemoveSharePointBinding"] = value;
				}
			}

			public virtual bool AllPublicFolderBindings
			{
				set
				{
					base.PowerSharpParameters["AllPublicFolderBindings"] = value;
				}
			}

			public virtual bool AllExchangeBindings
			{
				set
				{
					base.PowerSharpParameters["AllExchangeBindings"] = value;
				}
			}

			public virtual bool AllSharePointBindings
			{
				set
				{
					base.PowerSharpParameters["AllSharePointBindings"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
