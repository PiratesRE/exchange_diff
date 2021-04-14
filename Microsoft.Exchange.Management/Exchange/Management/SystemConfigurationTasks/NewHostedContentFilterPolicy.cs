using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "HostedContentFilterPolicy", SupportsShouldProcess = true)]
	public sealed class NewHostedContentFilterPolicy : NewMultitenancySystemConfigurationObjectTask<HostedContentFilterPolicy>
	{
		[Parameter]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = true, Position = 0)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter]
		public string AddXHeaderValue
		{
			get
			{
				return this.DataObject.AddXHeaderValue;
			}
			set
			{
				this.DataObject.AddXHeaderValue = value;
			}
		}

		[Parameter]
		public string ModifySubjectValue
		{
			get
			{
				return this.DataObject.ModifySubjectValue;
			}
			set
			{
				this.DataObject.ModifySubjectValue = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> RedirectToRecipients
		{
			get
			{
				return this.DataObject.RedirectToRecipients;
			}
			set
			{
				this.DataObject.RedirectToRecipients = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> TestModeBccToRecipients
		{
			get
			{
				return this.DataObject.TestModeBccToRecipients;
			}
			set
			{
				this.DataObject.TestModeBccToRecipients = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> FalsePositiveAdditionalRecipients
		{
			get
			{
				return this.DataObject.FalsePositiveAdditionalRecipients;
			}
			set
			{
				this.DataObject.FalsePositiveAdditionalRecipients = value;
			}
		}

		[Parameter]
		public SpamFilteringTestModeAction TestModeAction
		{
			get
			{
				return this.DataObject.TestModeAction;
			}
			set
			{
				this.DataObject.TestModeAction = value;
			}
		}

		[Parameter]
		public string AdminDisplayName
		{
			get
			{
				return this.DataObject.AdminDisplayName;
			}
			set
			{
				this.DataObject.AdminDisplayName = value;
			}
		}

		[Parameter]
		public SpamFilteringAction HighConfidenceSpamAction
		{
			get
			{
				return this.DataObject.HighConfidenceSpamAction;
			}
			set
			{
				this.DataObject.HighConfidenceSpamAction = value;
			}
		}

		[Parameter]
		public SpamFilteringAction SpamAction
		{
			get
			{
				return this.DataObject.SpamAction;
			}
			set
			{
				this.DataObject.SpamAction = value;
			}
		}

		[Parameter]
		public int QuarantineRetentionPeriod
		{
			get
			{
				return this.DataObject.QuarantineRetentionPeriod;
			}
			set
			{
				this.DataObject.QuarantineRetentionPeriod = value;
			}
		}

		[Parameter]
		public int EndUserSpamNotificationFrequency
		{
			get
			{
				return this.DataObject.EndUserSpamNotificationFrequency;
			}
			set
			{
				this.DataObject.EndUserSpamNotificationFrequency = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> LanguageBlockList
		{
			get
			{
				return this.DataObject.LanguageBlockList;
			}
			set
			{
				this.DataObject.LanguageBlockList = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RegionBlockList
		{
			get
			{
				return this.DataObject.RegionBlockList;
			}
			set
			{
				this.DataObject.RegionBlockList = value;
			}
		}

		[Parameter]
		public bool EnableEndUserSpamNotifications
		{
			get
			{
				return this.DataObject.EnableEndUserSpamNotifications;
			}
			set
			{
				this.DataObject.EnableEndUserSpamNotifications = value;
			}
		}

		[Parameter]
		public bool DownloadLink
		{
			get
			{
				return this.DataObject.DownloadLink;
			}
			set
			{
				this.DataObject.DownloadLink = value;
			}
		}

		[Parameter]
		public bool EnableRegionBlockList
		{
			get
			{
				return this.DataObject.EnableRegionBlockList;
			}
			set
			{
				this.DataObject.EnableRegionBlockList = value;
			}
		}

		[Parameter]
		public bool EnableLanguageBlockList
		{
			get
			{
				return this.DataObject.EnableLanguageBlockList;
			}
			set
			{
				this.DataObject.EnableLanguageBlockList = value;
			}
		}

		[Parameter]
		public SmtpAddress EndUserSpamNotificationCustomFromAddress
		{
			get
			{
				return this.DataObject.EndUserSpamNotificationCustomFromAddress;
			}
			set
			{
				this.DataObject.EndUserSpamNotificationCustomFromAddress = value;
			}
		}

		[Parameter]
		public string EndUserSpamNotificationCustomFromName
		{
			get
			{
				return this.DataObject.EndUserSpamNotificationCustomFromName;
			}
			set
			{
				this.DataObject.EndUserSpamNotificationCustomFromName = value;
			}
		}

		[Parameter]
		public string EndUserSpamNotificationCustomSubject
		{
			get
			{
				return this.DataObject.EndUserSpamNotificationCustomSubject;
			}
			set
			{
				this.DataObject.EndUserSpamNotificationCustomSubject = value;
			}
		}

		[Parameter]
		public EsnLanguage EndUserSpamNotificationLanguage
		{
			get
			{
				return this.DataObject.EndUserSpamNotificationLanguage;
			}
			set
			{
				this.DataObject.EndUserSpamNotificationLanguage = value;
			}
		}

		[Parameter]
		public int EndUserSpamNotificationLimit
		{
			get
			{
				return this.DataObject.EndUserSpamNotificationLimit;
			}
			set
			{
				this.DataObject.EndUserSpamNotificationLimit = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithImageLinks
		{
			get
			{
				return this.DataObject.IncreaseScoreWithImageLinks;
			}
			set
			{
				this.DataObject.IncreaseScoreWithImageLinks = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithNumericIps
		{
			get
			{
				return this.DataObject.IncreaseScoreWithNumericIps;
			}
			set
			{
				this.DataObject.IncreaseScoreWithNumericIps = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithRedirectToOtherPort
		{
			get
			{
				return this.DataObject.IncreaseScoreWithRedirectToOtherPort;
			}
			set
			{
				this.DataObject.IncreaseScoreWithRedirectToOtherPort = value;
			}
		}

		[Parameter]
		public SpamFilteringOption IncreaseScoreWithBizOrInfoUrls
		{
			get
			{
				return this.DataObject.IncreaseScoreWithBizOrInfoUrls;
			}
			set
			{
				this.DataObject.IncreaseScoreWithBizOrInfoUrls = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamEmptyMessages
		{
			get
			{
				return this.DataObject.MarkAsSpamEmptyMessages;
			}
			set
			{
				this.DataObject.MarkAsSpamEmptyMessages = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamJavaScriptInHtml
		{
			get
			{
				return this.DataObject.MarkAsSpamJavaScriptInHtml;
			}
			set
			{
				this.DataObject.MarkAsSpamJavaScriptInHtml = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFramesInHtml
		{
			get
			{
				return this.DataObject.MarkAsSpamFramesInHtml;
			}
			set
			{
				this.DataObject.MarkAsSpamFramesInHtml = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamObjectTagsInHtml
		{
			get
			{
				return this.DataObject.MarkAsSpamObjectTagsInHtml;
			}
			set
			{
				this.DataObject.MarkAsSpamObjectTagsInHtml = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamEmbedTagsInHtml
		{
			get
			{
				return this.DataObject.MarkAsSpamEmbedTagsInHtml;
			}
			set
			{
				this.DataObject.MarkAsSpamEmbedTagsInHtml = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFormTagsInHtml
		{
			get
			{
				return this.DataObject.MarkAsSpamFormTagsInHtml;
			}
			set
			{
				this.DataObject.MarkAsSpamFormTagsInHtml = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamWebBugsInHtml
		{
			get
			{
				return this.DataObject.MarkAsSpamWebBugsInHtml;
			}
			set
			{
				this.DataObject.MarkAsSpamWebBugsInHtml = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamSensitiveWordList
		{
			get
			{
				return this.DataObject.MarkAsSpamSensitiveWordList;
			}
			set
			{
				this.DataObject.MarkAsSpamSensitiveWordList = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamSpfRecordHardFail
		{
			get
			{
				return this.DataObject.MarkAsSpamSpfRecordHardFail;
			}
			set
			{
				this.DataObject.MarkAsSpamSpfRecordHardFail = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamFromAddressAuthFail
		{
			get
			{
				return this.DataObject.MarkAsSpamFromAddressAuthFail;
			}
			set
			{
				this.DataObject.MarkAsSpamFromAddressAuthFail = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamNdrBackscatter
		{
			get
			{
				return this.DataObject.MarkAsSpamNdrBackscatter;
			}
			set
			{
				this.DataObject.MarkAsSpamNdrBackscatter = value;
			}
		}

		[Parameter]
		public SpamFilteringOption MarkAsSpamBulkMail
		{
			get
			{
				return this.DataObject.MarkAsSpamBulkMail;
			}
			set
			{
				this.DataObject.MarkAsSpamBulkMail = value;
			}
		}

		[Parameter]
		public int BulkThreshold
		{
			get
			{
				return this.DataObject.BulkThreshold;
			}
			set
			{
				this.DataObject.BulkThreshold = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewHostedContentFilterPolicy(this.Name);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			HostedContentFilterPolicy hostedContentFilterPolicy = (HostedContentFilterPolicy)base.PrepareDataObject();
			hostedContentFilterPolicy.SetId((IConfigurationSession)base.DataSession, this.Name);
			if (!this.HostedContentFilterPolicyExist())
			{
				this.DataObject.IsDefault = true;
			}
			TaskLogger.LogExit();
			return hostedContentFilterPolicy;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.LanguageBlockList != null)
			{
				foreach (string text in from x in this.DataObject.LanguageBlockList
				where !HygieneUtils.IsAntispamFilterableLanguage(x)
				select x)
				{
					base.WriteError(new ArgumentException(Strings.ErrorUnsupportedBlockLanguage(text)), ErrorCategory.InvalidArgument, text);
				}
			}
			if (this.DataObject.RegionBlockList != null)
			{
				foreach (string text2 in from x in this.DataObject.RegionBlockList
				where !HygieneUtils.IsValidIso3166Alpha2Code(x)
				select x)
				{
					base.WriteError(new ArgumentException(Strings.ErrorInvalidIso3166Alpha2Code(text2)), ErrorCategory.InvalidArgument, text2);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.CreateParentContainerIfNeeded(this.DataObject);
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<HostedContentFilterPolicy>(this, this.DataObject, null);
			TaskLogger.LogExit();
		}

		private bool HostedContentFilterPolicyExist()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2012);
			HostedContentFilterPolicy[] array = ((IConfigurationSession)base.DataSession).Find<HostedContentFilterPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array.Length != 0;
		}
	}
}
