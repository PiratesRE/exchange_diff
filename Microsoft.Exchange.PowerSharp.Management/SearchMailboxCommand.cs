using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SearchMailboxCommand : SyntheticCommandWithPipelineInput<ADUser, ADUser>
	{
		private SearchMailboxCommand() : base("Search-Mailbox")
		{
		}

		public SearchMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SearchMailboxCommand SetParameters(SearchMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SearchMailboxCommand SetParameters(SearchMailboxCommand.MailboxParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SearchMailboxCommand SetParameters(SearchMailboxCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SearchMailboxCommand SetParameters(SearchMailboxCommand.EstimateResultParameters parameters)
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual string SearchQuery
			{
				set
				{
					base.PowerSharpParameters["SearchQuery"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpster
			{
				set
				{
					base.PowerSharpParameters["SearchDumpster"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpsterOnly
			{
				set
				{
					base.PowerSharpParameters["SearchDumpsterOnly"] = value;
				}
			}

			public virtual SwitchParameter IncludeUnsearchableItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnsearchableItems"] = value;
				}
			}

			public virtual SwitchParameter DoNotIncludeArchive
			{
				set
				{
					base.PowerSharpParameters["DoNotIncludeArchive"] = value;
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

		public class MailboxParameters : ParametersBase
		{
			public virtual string TargetMailbox
			{
				set
				{
					base.PowerSharpParameters["TargetMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string TargetFolder
			{
				set
				{
					base.PowerSharpParameters["TargetFolder"] = value;
				}
			}

			public virtual SwitchParameter DeleteContent
			{
				set
				{
					base.PowerSharpParameters["DeleteContent"] = value;
				}
			}

			public virtual LoggingLevel LogLevel
			{
				set
				{
					base.PowerSharpParameters["LogLevel"] = value;
				}
			}

			public virtual SwitchParameter LogOnly
			{
				set
				{
					base.PowerSharpParameters["LogOnly"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual string SearchQuery
			{
				set
				{
					base.PowerSharpParameters["SearchQuery"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpster
			{
				set
				{
					base.PowerSharpParameters["SearchDumpster"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpsterOnly
			{
				set
				{
					base.PowerSharpParameters["SearchDumpsterOnly"] = value;
				}
			}

			public virtual SwitchParameter IncludeUnsearchableItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnsearchableItems"] = value;
				}
			}

			public virtual SwitchParameter DoNotIncludeArchive
			{
				set
				{
					base.PowerSharpParameters["DoNotIncludeArchive"] = value;
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
			public virtual SwitchParameter DeleteContent
			{
				set
				{
					base.PowerSharpParameters["DeleteContent"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual string SearchQuery
			{
				set
				{
					base.PowerSharpParameters["SearchQuery"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpster
			{
				set
				{
					base.PowerSharpParameters["SearchDumpster"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpsterOnly
			{
				set
				{
					base.PowerSharpParameters["SearchDumpsterOnly"] = value;
				}
			}

			public virtual SwitchParameter IncludeUnsearchableItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnsearchableItems"] = value;
				}
			}

			public virtual SwitchParameter DoNotIncludeArchive
			{
				set
				{
					base.PowerSharpParameters["DoNotIncludeArchive"] = value;
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

		public class EstimateResultParameters : ParametersBase
		{
			public virtual SwitchParameter EstimateResultOnly
			{
				set
				{
					base.PowerSharpParameters["EstimateResultOnly"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual string SearchQuery
			{
				set
				{
					base.PowerSharpParameters["SearchQuery"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpster
			{
				set
				{
					base.PowerSharpParameters["SearchDumpster"] = value;
				}
			}

			public virtual SwitchParameter SearchDumpsterOnly
			{
				set
				{
					base.PowerSharpParameters["SearchDumpsterOnly"] = value;
				}
			}

			public virtual SwitchParameter IncludeUnsearchableItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnsearchableItems"] = value;
				}
			}

			public virtual SwitchParameter DoNotIncludeArchive
			{
				set
				{
					base.PowerSharpParameters["DoNotIncludeArchive"] = value;
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
