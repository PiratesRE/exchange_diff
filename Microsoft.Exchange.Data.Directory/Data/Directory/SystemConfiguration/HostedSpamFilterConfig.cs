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
	public class HostedSpamFilterConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return HostedSpamFilterConfig.SchemaObject;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return HostedSpamFilterConfig.HostedSpamFilteringContainer;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchHostedContentFilterConfig";
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.ObjectState == ObjectState.Changed)
			{
				if (this.MarkAsSpamSpfRecordHardFail == SpamFilteringOption.Test)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyCannotBeSetToTest, HostedSpamFilterConfigSchema.MarkAsSpamSpfRecordHardFail, this.MarkAsSpamSpfRecordHardFail));
				}
				if (this.MarkAsSpamNdrBackscatter == SpamFilteringOption.Test)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyCannotBeSetToTest, HostedSpamFilterConfigSchema.MarkAsSpamNdrBackscatter, this.MarkAsSpamNdrBackscatter));
				}
				if (this.MarkAsSpamFromAddressAuthFail == SpamFilteringOption.Test)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyCannotBeSetToTest, HostedSpamFilterConfigSchema.MarkAsSpamFromAddressAuthFail, this.MarkAsSpamFromAddressAuthFail));
				}
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
				return (string)this[HostedSpamFilterConfigSchema.AddXHeaderValue];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.AddXHeaderValue] = value;
			}
		}

		[Parameter]
		public string ModifySubjectValue
		{
			get
			{
				return (string)this[HostedSpamFilterConfigSchema.ModifySubjectValue];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.ModifySubjectValue] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> RedirectToRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedSpamFilterConfigSchema.RedirectToRecipients];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.RedirectToRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> TestModeBccToRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedSpamFilterConfigSchema.TestModeBccToRecipients];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.TestModeBccToRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> FalsePositiveAdditionalRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedSpamFilterConfigSchema.FalsePositiveAdditionalRecipients];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.FalsePositiveAdditionalRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> BccSuspiciousOutboundAdditionalRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedSpamFilterConfigSchema.BccSuspiciousOutboundAdditionalRecipients];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.BccSuspiciousOutboundAdditionalRecipients] = value;
			}
		}

		[ValidateRange(1, 30)]
		[Parameter]
		public int QuarantineRetentionPeriod
		{
			get
			{
				return (int)this[HostedSpamFilterConfigSchema.QuarantineRetentionPeriod];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.QuarantineRetentionPeriod] = value;
			}
		}

		[Parameter]
		[ValidateRange(3, 30)]
		public int DigestFrequency
		{
			get
			{
				return (int)this[HostedSpamFilterConfigSchema.DigestFrequency];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.DigestFrequency] = value;
			}
		}

		[Parameter]
		public SpamFilteringTestModeActions TestModeAction
		{
			get
			{
				return (SpamFilteringTestModeActions)this[HostedSpamFilterConfigSchema.TestModeAction];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.TestModeAction] = (int)value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithImageLinks
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.IncreaseScoreWithImageLinks];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.IncreaseScoreWithImageLinks] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithNumericIps
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.IncreaseScoreWithNumericIps];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.IncreaseScoreWithNumericIps] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithRedirectToOtherPort
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.IncreaseScoreWithRedirectToOtherPort];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.IncreaseScoreWithRedirectToOtherPort] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithBizOrInfoUrls
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.IncreaseScoreWithBizOrInfoUrls];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.IncreaseScoreWithBizOrInfoUrls] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamEmptyMessages
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamEmptyMessages];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamEmptyMessages] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamJavaScriptInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamJavaScriptInHtml];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamJavaScriptInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFramesInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamFramesInHtml];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamFramesInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamObjectTagsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamObjectTagsInHtml];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamObjectTagsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamEmbedTagsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamEmbedTagsInHtml];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamEmbedTagsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFormTagsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamFormTagsInHtml];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamFormTagsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamWebBugsInHtml
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamWebBugsInHtml];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamWebBugsInHtml] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamSensitiveWordList
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamSensitiveWordList];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamSensitiveWordList] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamSpfRecordHardFail
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamSpfRecordHardFail];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamSpfRecordHardFail] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFromAddressAuthFail
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamFromAddressAuthFail];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamFromAddressAuthFail] = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamNdrBackscatter
		{
			get
			{
				return (SpamFilteringOption)this[HostedSpamFilterConfigSchema.MarkAsSpamNdrBackscatter];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MarkAsSpamNdrBackscatter] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPRange> IPAllowList
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[HostedSpamFilterConfigSchema.IPAllowList];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.IPAllowList] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPRange> IPBlockList
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[HostedSpamFilterConfigSchema.IPBlockList];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.IPBlockList] = value;
			}
		}

		public bool IsDefault
		{
			get
			{
				return (bool)this[HostedSpamFilterConfigSchema.IsDefault];
			}
			internal set
			{
				this[HostedSpamFilterConfigSchema.IsDefault] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> NotifyOutboundSpamRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedSpamFilterConfigSchema.NotifyOutboundSpamRecipients];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.NotifyOutboundSpamRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> LanguageBlockList
		{
			get
			{
				return (MultiValuedProperty<string>)this[HostedSpamFilterConfigSchema.LanguageBlockList];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.LanguageBlockList] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> CountryBlockList
		{
			get
			{
				return (MultiValuedProperty<string>)this[HostedSpamFilterConfigSchema.CountryBlockList];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.CountryBlockList] = value;
			}
		}

		[Parameter]
		public SpamFilteringAction HighConfidenceAction
		{
			get
			{
				return (SpamFilteringAction)this[HostedSpamFilterConfigSchema.HighConfidenceAction];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.HighConfidenceAction] = (int)value;
			}
		}

		[Parameter]
		public SpamFilteringAction MediumConfidenceAction
		{
			get
			{
				return (SpamFilteringAction)this[HostedSpamFilterConfigSchema.MediumConfidenceAction];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MediumConfidenceAction] = (int)value;
			}
		}

		[Parameter]
		public SpamFilteringAction LowConfidenceAction
		{
			get
			{
				return (SpamFilteringAction)this[HostedSpamFilterConfigSchema.LowConfidenceAction];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.LowConfidenceAction] = (int)value;
			}
		}

		[Parameter]
		public bool BccSuspiciousOutboundMail
		{
			get
			{
				return (bool)this[HostedSpamFilterConfigSchema.BccSuspiciousOutboundMail];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.BccSuspiciousOutboundMail] = value;
			}
		}

		[Parameter]
		public bool NotifyOutboundSpam
		{
			get
			{
				return (bool)this[HostedSpamFilterConfigSchema.NotifyOutboundSpam];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.NotifyOutboundSpam] = value;
			}
		}

		[Parameter]
		public bool MoveToJmfEnableHostedQuarantine
		{
			get
			{
				return (bool)this[HostedSpamFilterConfigSchema.MoveToJmfEnableHostedQuarantine];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.MoveToJmfEnableHostedQuarantine] = value;
			}
		}

		[Parameter]
		public bool EnableDigests
		{
			get
			{
				return (bool)this[HostedSpamFilterConfigSchema.EnableDigests];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.EnableDigests] = value;
			}
		}

		[Parameter]
		public bool DownloadLink
		{
			get
			{
				return (bool)this[HostedSpamFilterConfigSchema.DownloadLink];
			}
			set
			{
				this[HostedSpamFilterConfigSchema.DownloadLink] = value;
			}
		}

		internal const string LdapName = "msExchHostedContentFilterConfig";

		internal static readonly ADObjectId HostedSpamFilteringContainer = new ADObjectId("CN=Hosted Spam Filtering,CN=Transport Settings");

		private static readonly HostedSpamFilterConfigSchema SchemaObject = ObjectSchema.GetInstance<HostedSpamFilterConfigSchema>();
	}
}
