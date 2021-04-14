using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxSearchCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxDiscoverySearch>
	{
		private SetMailboxSearchCommand() : base("Set-MailboxSearch")
		{
		}

		public SetMailboxSearchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxSearchCommand SetParameters(SetMailboxSearchCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxSearchCommand SetParameters(SetMailboxSearchCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string SearchQuery
			{
				set
				{
					base.PowerSharpParameters["SearchQuery"] = value;
				}
			}

			public virtual RecipientIdParameter SourceMailboxes
			{
				set
				{
					base.PowerSharpParameters["SourceMailboxes"] = value;
				}
			}

			public virtual string TargetMailbox
			{
				set
				{
					base.PowerSharpParameters["TargetMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual PublicFolderIdParameter PublicFolderSources
			{
				set
				{
					base.PowerSharpParameters["PublicFolderSources"] = value;
				}
			}

			public virtual bool AllPublicFolderSources
			{
				set
				{
					base.PowerSharpParameters["AllPublicFolderSources"] = value;
				}
			}

			public virtual bool AllSourceMailboxes
			{
				set
				{
					base.PowerSharpParameters["AllSourceMailboxes"] = value;
				}
			}

			public virtual string Senders
			{
				set
				{
					base.PowerSharpParameters["Senders"] = value;
				}
			}

			public virtual string Recipients
			{
				set
				{
					base.PowerSharpParameters["Recipients"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual KindKeyword MessageTypes
			{
				set
				{
					base.PowerSharpParameters["MessageTypes"] = value;
				}
			}

			public virtual RecipientIdParameter StatusMailRecipients
			{
				set
				{
					base.PowerSharpParameters["StatusMailRecipients"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual bool EstimateOnly
			{
				set
				{
					base.PowerSharpParameters["EstimateOnly"] = value;
				}
			}

			public virtual SwitchParameter IncludeKeywordStatistics
			{
				set
				{
					base.PowerSharpParameters["IncludeKeywordStatistics"] = value;
				}
			}

			public virtual int StatisticsStartIndex
			{
				set
				{
					base.PowerSharpParameters["StatisticsStartIndex"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IncludeUnsearchableItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnsearchableItems"] = value;
				}
			}

			public virtual LoggingLevel LogLevel
			{
				set
				{
					base.PowerSharpParameters["LogLevel"] = value;
				}
			}

			public virtual string Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual bool ExcludeDuplicateMessages
			{
				set
				{
					base.PowerSharpParameters["ExcludeDuplicateMessages"] = value;
				}
			}

			public virtual bool InPlaceHoldEnabled
			{
				set
				{
					base.PowerSharpParameters["InPlaceHoldEnabled"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> ItemHoldPeriod
			{
				set
				{
					base.PowerSharpParameters["ItemHoldPeriod"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new EwsStoreObjectIdParameter(value) : null);
				}
			}

			public virtual string SearchQuery
			{
				set
				{
					base.PowerSharpParameters["SearchQuery"] = value;
				}
			}

			public virtual RecipientIdParameter SourceMailboxes
			{
				set
				{
					base.PowerSharpParameters["SourceMailboxes"] = value;
				}
			}

			public virtual string TargetMailbox
			{
				set
				{
					base.PowerSharpParameters["TargetMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual PublicFolderIdParameter PublicFolderSources
			{
				set
				{
					base.PowerSharpParameters["PublicFolderSources"] = value;
				}
			}

			public virtual bool AllPublicFolderSources
			{
				set
				{
					base.PowerSharpParameters["AllPublicFolderSources"] = value;
				}
			}

			public virtual bool AllSourceMailboxes
			{
				set
				{
					base.PowerSharpParameters["AllSourceMailboxes"] = value;
				}
			}

			public virtual string Senders
			{
				set
				{
					base.PowerSharpParameters["Senders"] = value;
				}
			}

			public virtual string Recipients
			{
				set
				{
					base.PowerSharpParameters["Recipients"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual KindKeyword MessageTypes
			{
				set
				{
					base.PowerSharpParameters["MessageTypes"] = value;
				}
			}

			public virtual RecipientIdParameter StatusMailRecipients
			{
				set
				{
					base.PowerSharpParameters["StatusMailRecipients"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual bool EstimateOnly
			{
				set
				{
					base.PowerSharpParameters["EstimateOnly"] = value;
				}
			}

			public virtual SwitchParameter IncludeKeywordStatistics
			{
				set
				{
					base.PowerSharpParameters["IncludeKeywordStatistics"] = value;
				}
			}

			public virtual int StatisticsStartIndex
			{
				set
				{
					base.PowerSharpParameters["StatisticsStartIndex"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IncludeUnsearchableItems
			{
				set
				{
					base.PowerSharpParameters["IncludeUnsearchableItems"] = value;
				}
			}

			public virtual LoggingLevel LogLevel
			{
				set
				{
					base.PowerSharpParameters["LogLevel"] = value;
				}
			}

			public virtual string Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual bool ExcludeDuplicateMessages
			{
				set
				{
					base.PowerSharpParameters["ExcludeDuplicateMessages"] = value;
				}
			}

			public virtual bool InPlaceHoldEnabled
			{
				set
				{
					base.PowerSharpParameters["InPlaceHoldEnabled"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> ItemHoldPeriod
			{
				set
				{
					base.PowerSharpParameters["ItemHoldPeriod"] = value;
				}
			}

			public virtual string Description
			{
				set
				{
					base.PowerSharpParameters["Description"] = value;
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
