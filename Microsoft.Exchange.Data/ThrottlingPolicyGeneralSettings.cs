using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyGeneralSettings : ThrottlingPolicyBaseSettings
	{
		public ThrottlingPolicyGeneralSettings()
		{
		}

		private ThrottlingPolicyGeneralSettings(string value) : base(value)
		{
			Unlimited<uint>? messageRateLimit = this.MessageRateLimit;
			Unlimited<uint>? recipientRateLimit = this.RecipientRateLimit;
			Unlimited<uint>? forwardeeLimit = this.ForwardeeLimit;
			Unlimited<uint>? discoveryMaxConcurrency = this.DiscoveryMaxConcurrency;
			Unlimited<uint>? discoveryMaxMailboxes = this.DiscoveryMaxMailboxes;
			Unlimited<uint>? discoveryMaxKeywords = this.DiscoveryMaxKeywords;
			Unlimited<uint>? discoveryMaxPreviewSearchMailboxes = this.DiscoveryMaxPreviewSearchMailboxes;
			Unlimited<uint>? discoveryMaxStatsSearchMailboxes = this.DiscoveryMaxStatsSearchMailboxes;
			Unlimited<uint>? discoveryPreviewSearchResultsPageSize = this.DiscoveryPreviewSearchResultsPageSize;
			Unlimited<uint>? discoveryMaxKeywordsPerPage = this.DiscoveryMaxKeywordsPerPage;
			Unlimited<uint>? discoveryMaxRefinerResults = this.DiscoveryMaxRefinerResults;
			Unlimited<uint>? discoveryMaxSearchQueueDepth = this.DiscoveryMaxSearchQueueDepth;
			Unlimited<uint>? discoverySearchTimeoutPeriod = this.DiscoverySearchTimeoutPeriod;
			Unlimited<uint>? complianceMaxExpansionDGRecipients = this.ComplianceMaxExpansionDGRecipients;
			Unlimited<uint>? complianceMaxExpansionNestedDGs = this.ComplianceMaxExpansionNestedDGs;
		}

		public static ThrottlingPolicyGeneralSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyGeneralSettings(stateToParse);
		}

		internal Unlimited<uint>? MessageRateLimit
		{
			get
			{
				return base.GetValueFromPropertyBag("MsgRateLimit");
			}
			set
			{
				base.SetValueInPropertyBag("MsgRateLimit", value);
			}
		}

		internal Unlimited<uint>? RecipientRateLimit
		{
			get
			{
				return base.GetValueFromPropertyBag("RecipRateLimit");
			}
			set
			{
				base.SetValueInPropertyBag("RecipRateLimit", value);
			}
		}

		internal Unlimited<uint>? ForwardeeLimit
		{
			get
			{
				return base.GetValueFromPropertyBag("ForwardeeLimit");
			}
			set
			{
				base.SetValueInPropertyBag("ForwardeeLimit", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxConcurr");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxConcurr", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxMailboxes
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxMailboxes");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxMailboxes", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxKeywords
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxKeywords");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxKeywords", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxPreviewMailboxes");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxPreviewMailboxes", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxStatsMailboxes");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxStatsMailboxes", value);
			}
		}

		internal Unlimited<uint>? DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryPreviewSearchResultsPageSize");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryPreviewSearchResultsPageSize", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxKeywordsPerPage");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxKeywordsPerPage", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxRefinerResults
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxRefinerResults");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxRefinerResults", value);
			}
		}

		internal Unlimited<uint>? DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoveryMaxSearchQueueDepth");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoveryMaxSearchQueueDepth", value);
			}
		}

		internal Unlimited<uint>? DiscoverySearchTimeoutPeriod
		{
			get
			{
				return base.GetValueFromPropertyBag("DiscoverySearchTimeoutPeriod");
			}
			set
			{
				base.SetValueInPropertyBag("DiscoverySearchTimeoutPeriod", value);
			}
		}

		internal Unlimited<uint>? ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return base.GetValueFromPropertyBag("ComplianceMaxExpansionDGRecipients");
			}
			set
			{
				base.SetValueInPropertyBag("ComplianceMaxExpansionDGRecipients", value);
			}
		}

		internal Unlimited<uint>? ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return base.GetValueFromPropertyBag("ComplianceMaxExpansionNestedDGs");
			}
			set
			{
				base.SetValueInPropertyBag("ComplianceMaxExpansionNestedDGs", value);
			}
		}

		private const string MessageRateLimitPrefix = "MsgRateLimit";

		private const string RecipientRateLimitPrefix = "RecipRateLimit";

		private const string ForwardeeLimitPrefix = "ForwardeeLimit";

		private const string DiscoveryMaxConcurrencyPrefix = "DiscoveryMaxConcurr";

		private const string DiscoveryMaxMailboxesPrefix = "DiscoveryMaxMailboxes";

		private const string DiscoveryMaxPreviewSearchMailboxesPrefix = "DiscoveryMaxPreviewMailboxes";

		private const string DiscoveryMaxStatsSearchMailboxesPrefix = "DiscoveryMaxStatsMailboxes";

		private const string DiscoveryMaxKeywordsPrefix = "DiscoveryMaxKeywords";

		private const string DiscoveryPreviewSearchResultsPageSizePrefix = "DiscoveryPreviewSearchResultsPageSize";

		private const string DiscoveryMaxKeywordsPerPagePrefix = "DiscoveryMaxKeywordsPerPage";

		private const string DiscoveryMaxRefinerResultsPrefix = "DiscoveryMaxRefinerResults";

		private const string DiscoveryMaxSearchQueueDepthPrefix = "DiscoveryMaxSearchQueueDepth";

		private const string DiscoverySearchTimeoutPeriodPrefix = "DiscoverySearchTimeoutPeriod";

		private const string ComplianceMaxExpansionDGRecipientsPrefix = "ComplianceMaxExpansionDGRecipients";

		private const string ComplianceMaxExpansionNestedDGsPrefix = "ComplianceMaxExpansionNestedDGs";
	}
}
