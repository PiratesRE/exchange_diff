using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class HostedContentFilterPolicy : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return HostedContentFilterPolicy.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return HostedContentFilterPolicy.parentPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return HostedContentFilterPolicy.ldapName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.MarkAsSpamSpfRecordHardFail == SpamFilteringOption.Test)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.PropertyCannotBeSetToTest, HostedContentFilterPolicySchema.MarkAsSpamSpfRecordHardFail, this.MarkAsSpamSpfRecordHardFail));
			}
			if (this.MarkAsSpamNdrBackscatter == SpamFilteringOption.Test)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.PropertyCannotBeSetToTest, HostedContentFilterPolicySchema.MarkAsSpamNdrBackscatter, this.MarkAsSpamNdrBackscatter));
			}
			if (this.MarkAsSpamFromAddressAuthFail == SpamFilteringOption.Test)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.PropertyCannotBeSetToTest, HostedContentFilterPolicySchema.MarkAsSpamFromAddressAuthFail, this.MarkAsSpamFromAddressAuthFail));
			}
			if ((this.HighConfidenceSpamAction == SpamFilteringAction.AddXHeader || this.SpamAction == SpamFilteringAction.AddXHeader) && string.IsNullOrEmpty(this.AddXHeaderValue))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.XHeaderValueNotSet, HostedContentFilterPolicySchema.AddXHeaderValue, this.AddXHeaderValue));
			}
			if ((this.HighConfidenceSpamAction == SpamFilteringAction.ModifySubject || this.SpamAction == SpamFilteringAction.ModifySubject) && string.IsNullOrEmpty(this.ModifySubjectValue))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ModifySubjectValueNotSet, HostedContentFilterPolicySchema.ModifySubjectValue, this.ModifySubjectValue));
			}
			if ((this.HighConfidenceSpamAction == SpamFilteringAction.Redirect || this.SpamAction == SpamFilteringAction.Redirect) && (this.RedirectToRecipients == null || this.RedirectToRecipients.Count == 0))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.RedirectToRecipientsNotSet, HostedContentFilterPolicySchema.RedirectToRecipients, this.RedirectToRecipients));
			}
			if (this.EnableLanguageBlockList && (this.LanguageBlockList == null || this.LanguageBlockList.Count == 0))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.LanguageBlockListNotSet, HostedContentFilterPolicySchema.LanguageBlockList, this.LanguageBlockList));
			}
			if (this.EnableRegionBlockList && (this.RegionBlockList == null || this.RegionBlockList.Count == 0))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.RegionBlockListNotSet, HostedContentFilterPolicySchema.RegionBlockList, this.RegionBlockList));
			}
		}

		[Parameter]
		public new string AdminDisplayName
		{
			get
			{
				return (string)this[ADConfigurationObjectSchema.AdminDisplayName];
			}
			set
			{
				this[ADConfigurationObjectSchema.AdminDisplayName] = value;
			}
		}

		[Parameter]
		public string AddXHeaderValue
		{
			get
			{
				return (string)this[HostedContentFilterPolicySchema.AddXHeaderValue];
			}
			set
			{
				this[HostedContentFilterPolicySchema.AddXHeaderValue] = value;
			}
		}

		[Parameter]
		public string ModifySubjectValue
		{
			get
			{
				return (string)this[HostedContentFilterPolicySchema.ModifySubjectValue];
			}
			set
			{
				this[HostedContentFilterPolicySchema.ModifySubjectValue] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> RedirectToRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedContentFilterPolicySchema.RedirectToRecipients];
			}
			set
			{
				this[HostedContentFilterPolicySchema.RedirectToRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> TestModeBccToRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedContentFilterPolicySchema.TestModeBccToRecipients];
			}
			set
			{
				this[HostedContentFilterPolicySchema.TestModeBccToRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> FalsePositiveAdditionalRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedContentFilterPolicySchema.FalsePositiveAdditionalRecipients];
			}
			set
			{
				this[HostedContentFilterPolicySchema.FalsePositiveAdditionalRecipients] = value;
			}
		}

		[Parameter]
		public int QuarantineRetentionPeriod
		{
			get
			{
				return (int)this[HostedContentFilterPolicySchema.QuarantineRetentionPeriod];
			}
			set
			{
				this[HostedContentFilterPolicySchema.QuarantineRetentionPeriod] = value;
			}
		}

		[Parameter]
		public int EndUserSpamNotificationFrequency
		{
			get
			{
				return (int)this[HostedContentFilterPolicySchema.EndUserSpamNotificationFrequency];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EndUserSpamNotificationFrequency] = value;
			}
		}

		[Parameter]
		public SpamFilteringTestModeAction TestModeAction
		{
			get
			{
				return (SpamFilteringTestModeAction)this[HostedContentFilterPolicySchema.TestModeAction];
			}
			set
			{
				this[HostedContentFilterPolicySchema.TestModeAction] = (int)value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithImageLinks
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.IncreaseScoreWithImageLinks];
			}
			set
			{
				this[HostedContentFilterPolicySchema.IncreaseScoreWithImageLinks] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithNumericIps
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.IncreaseScoreWithNumericIps];
			}
			set
			{
				this[HostedContentFilterPolicySchema.IncreaseScoreWithNumericIps] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithRedirectToOtherPort
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.IncreaseScoreWithRedirectToOtherPort];
			}
			set
			{
				this[HostedContentFilterPolicySchema.IncreaseScoreWithRedirectToOtherPort] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithBizOrInfoUrls
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.IncreaseScoreWithBizOrInfoUrls];
			}
			set
			{
				this[HostedContentFilterPolicySchema.IncreaseScoreWithBizOrInfoUrls] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamEmptyMessages
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamEmptyMessages];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamEmptyMessages] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamJavaScriptInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamJavaScriptInHtml];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamJavaScriptInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFramesInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamFramesInHtml];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamFramesInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamObjectTagsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamObjectTagsInHtml];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamObjectTagsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamEmbedTagsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamEmbedTagsInHtml];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamEmbedTagsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFormTagsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamFormTagsInHtml];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamFormTagsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamWebBugsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamWebBugsInHtml];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamWebBugsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamSensitiveWordList
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamSensitiveWordList];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamSensitiveWordList] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamSpfRecordHardFail
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamSpfRecordHardFail];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamSpfRecordHardFail] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFromAddressAuthFail
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamFromAddressAuthFail];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamFromAddressAuthFail] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamBulkMail
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamBulkMail];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamBulkMail] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamNdrBackscatter
		{
			get
			{
				return (SpamFilteringOption)this[HostedContentFilterPolicySchema.MarkAsSpamNdrBackscatter];
			}
			set
			{
				this[HostedContentFilterPolicySchema.MarkAsSpamNdrBackscatter] = value;
			}
		}

		public bool IsDefault
		{
			get
			{
				return (bool)this[HostedContentFilterPolicySchema.IsDefault];
			}
			internal set
			{
				this[HostedContentFilterPolicySchema.IsDefault] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> LanguageBlockList
		{
			get
			{
				return (MultiValuedProperty<string>)this[HostedContentFilterPolicySchema.LanguageBlockList];
			}
			set
			{
				this[HostedContentFilterPolicySchema.LanguageBlockList] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RegionBlockList
		{
			get
			{
				return (MultiValuedProperty<string>)this[HostedContentFilterPolicySchema.RegionBlockList];
			}
			set
			{
				this[HostedContentFilterPolicySchema.RegionBlockList] = value;
			}
		}

		[Parameter]
		public SpamFilteringAction HighConfidenceSpamAction
		{
			get
			{
				return (SpamFilteringAction)this[HostedContentFilterPolicySchema.HighConfidenceSpamAction];
			}
			set
			{
				this[HostedContentFilterPolicySchema.HighConfidenceSpamAction] = (int)value;
			}
		}

		[Parameter]
		public SpamFilteringAction SpamAction
		{
			get
			{
				return (SpamFilteringAction)this[HostedContentFilterPolicySchema.SpamAction];
			}
			set
			{
				this[HostedContentFilterPolicySchema.SpamAction] = (int)value;
			}
		}

		[Parameter]
		public bool EnableEndUserSpamNotifications
		{
			get
			{
				return (bool)this[HostedContentFilterPolicySchema.EnableEndUserSpamNotifications];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EnableEndUserSpamNotifications] = value;
			}
		}

		[Parameter]
		public bool DownloadLink
		{
			get
			{
				return (bool)this[HostedContentFilterPolicySchema.DownloadLink];
			}
			set
			{
				this[HostedContentFilterPolicySchema.DownloadLink] = value;
			}
		}

		[Parameter]
		public bool EnableRegionBlockList
		{
			get
			{
				return (bool)this[HostedContentFilterPolicySchema.EnableRegionBlockList];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EnableRegionBlockList] = value;
			}
		}

		[Parameter]
		public bool EnableLanguageBlockList
		{
			get
			{
				return (bool)this[HostedContentFilterPolicySchema.EnableLanguageBlockList];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EnableLanguageBlockList] = value;
			}
		}

		[Parameter]
		public SmtpAddress EndUserSpamNotificationCustomFromAddress
		{
			get
			{
				return (SmtpAddress)this[HostedContentFilterPolicySchema.EndUserSpamNotificationCustomFromAddress];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EndUserSpamNotificationCustomFromAddress] = value;
			}
		}

		[Parameter]
		public string EndUserSpamNotificationCustomFromName
		{
			get
			{
				return (string)this[HostedContentFilterPolicySchema.EndUserSpamNotificationCustomFromName];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EndUserSpamNotificationCustomFromName] = value;
			}
		}

		[Parameter]
		public string EndUserSpamNotificationCustomSubject
		{
			get
			{
				return (string)this[HostedContentFilterPolicySchema.EndUserSpamNotificationCustomSubject];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EndUserSpamNotificationCustomSubject] = value;
			}
		}

		[Parameter]
		public EsnLanguage EndUserSpamNotificationLanguage
		{
			get
			{
				return (EsnLanguage)this[HostedContentFilterPolicySchema.EndUserSpamNotificationLanguage];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EndUserSpamNotificationLanguage] = value;
			}
		}

		[Parameter]
		public int EndUserSpamNotificationLimit
		{
			get
			{
				return (int)this[HostedContentFilterPolicySchema.EndUserSpamNotificationLimit];
			}
			set
			{
				this[HostedContentFilterPolicySchema.EndUserSpamNotificationLimit] = value;
			}
		}

		[Parameter]
		public int BulkThreshold
		{
			get
			{
				return (int)this[HostedContentFilterPolicySchema.BulkThreshold];
			}
			set
			{
				this[HostedContentFilterPolicySchema.BulkThreshold] = value;
			}
		}

		internal bool IsConflicted()
		{
			return ADSession.IsCNFObject(base.Id);
		}

		private static readonly string ldapName = "msExchHostedContentFilterConfig";

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Hosted Content Filter,CN=Transport Settings");

		private static readonly HostedContentFilterPolicySchema schema = ObjectSchema.GetInstance<HostedContentFilterPolicySchema>();
	}
}
