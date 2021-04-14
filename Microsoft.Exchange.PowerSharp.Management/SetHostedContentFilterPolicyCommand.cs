using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHostedContentFilterPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<HostedContentFilterPolicy>
	{
		private SetHostedContentFilterPolicyCommand() : base("Set-HostedContentFilterPolicy")
		{
		}

		public SetHostedContentFilterPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHostedContentFilterPolicyCommand SetParameters(SetHostedContentFilterPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetHostedContentFilterPolicyCommand SetParameters(SetHostedContentFilterPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter MakeDefault
			{
				set
				{
					base.PowerSharpParameters["MakeDefault"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string AdminDisplayName
			{
				set
				{
					base.PowerSharpParameters["AdminDisplayName"] = value;
				}
			}

			public virtual string AddXHeaderValue
			{
				set
				{
					base.PowerSharpParameters["AddXHeaderValue"] = value;
				}
			}

			public virtual string ModifySubjectValue
			{
				set
				{
					base.PowerSharpParameters["ModifySubjectValue"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> RedirectToRecipients
			{
				set
				{
					base.PowerSharpParameters["RedirectToRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> TestModeBccToRecipients
			{
				set
				{
					base.PowerSharpParameters["TestModeBccToRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> FalsePositiveAdditionalRecipients
			{
				set
				{
					base.PowerSharpParameters["FalsePositiveAdditionalRecipients"] = value;
				}
			}

			public virtual int QuarantineRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["QuarantineRetentionPeriod"] = value;
				}
			}

			public virtual int EndUserSpamNotificationFrequency
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationFrequency"] = value;
				}
			}

			public virtual SpamFilteringTestModeAction TestModeAction
			{
				set
				{
					base.PowerSharpParameters["TestModeAction"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithImageLinks
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithImageLinks"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithNumericIps
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithNumericIps"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithRedirectToOtherPort
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithRedirectToOtherPort"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithBizOrInfoUrls
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithBizOrInfoUrls"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamEmptyMessages
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamEmptyMessages"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamJavaScriptInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamJavaScriptInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamFramesInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamFramesInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamObjectTagsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamObjectTagsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamEmbedTagsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamEmbedTagsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamFormTagsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamFormTagsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamWebBugsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamWebBugsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamSensitiveWordList
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamSensitiveWordList"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamSpfRecordHardFail
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamSpfRecordHardFail"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamFromAddressAuthFail
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamFromAddressAuthFail"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamBulkMail
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamBulkMail"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamNdrBackscatter
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamNdrBackscatter"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LanguageBlockList
			{
				set
				{
					base.PowerSharpParameters["LanguageBlockList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RegionBlockList
			{
				set
				{
					base.PowerSharpParameters["RegionBlockList"] = value;
				}
			}

			public virtual SpamFilteringAction HighConfidenceSpamAction
			{
				set
				{
					base.PowerSharpParameters["HighConfidenceSpamAction"] = value;
				}
			}

			public virtual SpamFilteringAction SpamAction
			{
				set
				{
					base.PowerSharpParameters["SpamAction"] = value;
				}
			}

			public virtual bool EnableEndUserSpamNotifications
			{
				set
				{
					base.PowerSharpParameters["EnableEndUserSpamNotifications"] = value;
				}
			}

			public virtual bool DownloadLink
			{
				set
				{
					base.PowerSharpParameters["DownloadLink"] = value;
				}
			}

			public virtual bool EnableRegionBlockList
			{
				set
				{
					base.PowerSharpParameters["EnableRegionBlockList"] = value;
				}
			}

			public virtual bool EnableLanguageBlockList
			{
				set
				{
					base.PowerSharpParameters["EnableLanguageBlockList"] = value;
				}
			}

			public virtual SmtpAddress EndUserSpamNotificationCustomFromAddress
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationCustomFromAddress"] = value;
				}
			}

			public virtual string EndUserSpamNotificationCustomFromName
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationCustomFromName"] = value;
				}
			}

			public virtual string EndUserSpamNotificationCustomSubject
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationCustomSubject"] = value;
				}
			}

			public virtual EsnLanguage EndUserSpamNotificationLanguage
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationLanguage"] = value;
				}
			}

			public virtual int EndUserSpamNotificationLimit
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationLimit"] = value;
				}
			}

			public virtual int BulkThreshold
			{
				set
				{
					base.PowerSharpParameters["BulkThreshold"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new HostedContentFilterPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter MakeDefault
			{
				set
				{
					base.PowerSharpParameters["MakeDefault"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string AdminDisplayName
			{
				set
				{
					base.PowerSharpParameters["AdminDisplayName"] = value;
				}
			}

			public virtual string AddXHeaderValue
			{
				set
				{
					base.PowerSharpParameters["AddXHeaderValue"] = value;
				}
			}

			public virtual string ModifySubjectValue
			{
				set
				{
					base.PowerSharpParameters["ModifySubjectValue"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> RedirectToRecipients
			{
				set
				{
					base.PowerSharpParameters["RedirectToRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> TestModeBccToRecipients
			{
				set
				{
					base.PowerSharpParameters["TestModeBccToRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> FalsePositiveAdditionalRecipients
			{
				set
				{
					base.PowerSharpParameters["FalsePositiveAdditionalRecipients"] = value;
				}
			}

			public virtual int QuarantineRetentionPeriod
			{
				set
				{
					base.PowerSharpParameters["QuarantineRetentionPeriod"] = value;
				}
			}

			public virtual int EndUserSpamNotificationFrequency
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationFrequency"] = value;
				}
			}

			public virtual SpamFilteringTestModeAction TestModeAction
			{
				set
				{
					base.PowerSharpParameters["TestModeAction"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithImageLinks
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithImageLinks"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithNumericIps
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithNumericIps"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithRedirectToOtherPort
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithRedirectToOtherPort"] = value;
				}
			}

			public virtual SpamFilteringOption IncreaseScoreWithBizOrInfoUrls
			{
				set
				{
					base.PowerSharpParameters["IncreaseScoreWithBizOrInfoUrls"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamEmptyMessages
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamEmptyMessages"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamJavaScriptInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamJavaScriptInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamFramesInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamFramesInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamObjectTagsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamObjectTagsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamEmbedTagsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamEmbedTagsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamFormTagsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamFormTagsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamWebBugsInHtml
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamWebBugsInHtml"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamSensitiveWordList
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamSensitiveWordList"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamSpfRecordHardFail
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamSpfRecordHardFail"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamFromAddressAuthFail
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamFromAddressAuthFail"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamBulkMail
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamBulkMail"] = value;
				}
			}

			public virtual SpamFilteringOption MarkAsSpamNdrBackscatter
			{
				set
				{
					base.PowerSharpParameters["MarkAsSpamNdrBackscatter"] = value;
				}
			}

			public virtual MultiValuedProperty<string> LanguageBlockList
			{
				set
				{
					base.PowerSharpParameters["LanguageBlockList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RegionBlockList
			{
				set
				{
					base.PowerSharpParameters["RegionBlockList"] = value;
				}
			}

			public virtual SpamFilteringAction HighConfidenceSpamAction
			{
				set
				{
					base.PowerSharpParameters["HighConfidenceSpamAction"] = value;
				}
			}

			public virtual SpamFilteringAction SpamAction
			{
				set
				{
					base.PowerSharpParameters["SpamAction"] = value;
				}
			}

			public virtual bool EnableEndUserSpamNotifications
			{
				set
				{
					base.PowerSharpParameters["EnableEndUserSpamNotifications"] = value;
				}
			}

			public virtual bool DownloadLink
			{
				set
				{
					base.PowerSharpParameters["DownloadLink"] = value;
				}
			}

			public virtual bool EnableRegionBlockList
			{
				set
				{
					base.PowerSharpParameters["EnableRegionBlockList"] = value;
				}
			}

			public virtual bool EnableLanguageBlockList
			{
				set
				{
					base.PowerSharpParameters["EnableLanguageBlockList"] = value;
				}
			}

			public virtual SmtpAddress EndUserSpamNotificationCustomFromAddress
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationCustomFromAddress"] = value;
				}
			}

			public virtual string EndUserSpamNotificationCustomFromName
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationCustomFromName"] = value;
				}
			}

			public virtual string EndUserSpamNotificationCustomSubject
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationCustomSubject"] = value;
				}
			}

			public virtual EsnLanguage EndUserSpamNotificationLanguage
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationLanguage"] = value;
				}
			}

			public virtual int EndUserSpamNotificationLimit
			{
				set
				{
					base.PowerSharpParameters["EndUserSpamNotificationLimit"] = value;
				}
			}

			public virtual int BulkThreshold
			{
				set
				{
					base.PowerSharpParameters["BulkThreshold"] = value;
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
