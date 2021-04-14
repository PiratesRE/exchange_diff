using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips;
using Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class MailTipsConfiguration
	{
		public MailTipsConfiguration(int traceId)
		{
			this.traceId = traceId;
		}

		public MailTipsConfiguration(int maxMessageSize, int largeAudienceThreshold, bool showExternalRecipientCount, bool policyTipsEnabled)
		{
			this.maxMessageSize = maxMessageSize;
			this.largeAudienceThreshold = largeAudienceThreshold;
			this.showExternalRecipientCount = showExternalRecipientCount;
			this.policyTipsEnabled = policyTipsEnabled;
		}

		public int MaxMessageSize
		{
			get
			{
				return this.maxMessageSize;
			}
		}

		public int LargeAudienceThreshold
		{
			get
			{
				return this.largeAudienceThreshold;
			}
		}

		public bool ShowExternalRecipientCount
		{
			get
			{
				return this.showExternalRecipientCount;
			}
		}

		public bool PolicyTipsEnabled
		{
			get
			{
				return this.policyTipsEnabled;
			}
		}

		public void Initialize(CachedOrganizationConfiguration configuration, ADRawEntry sender)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			MailTipsConfiguration.GetMailTipsConfigurationTracer.TraceFunction<CachedOrganizationConfiguration, string>((long)this.traceId, "Entering MailTipsConfiguration.Initialize({0}, {1})", configuration, sender.Id.ToString());
			this.DetermineMaxMessageSize(configuration.TransportSettings.Configuration, sender);
			Organization configuration2 = configuration.OrganizationConfiguration.Configuration;
			this.showExternalRecipientCount = configuration2.MailTipsExternalRecipientsTipsEnabled;
			this.largeAudienceThreshold = (int)configuration2.MailTipsLargeAudienceThreshold;
			this.policyTipsEnabled = (from rule in configuration.PolicyNudgeRules.Rules
			where rule.IsEnabled
			select rule).Any<PolicyNudgeRule>();
		}

		private void DetermineMaxMessageSize(TransportConfigContainer transportConfiguration, ADRawEntry sender)
		{
			if (transportConfiguration.MaxSendSize.IsUnlimited)
			{
				this.maxMessageSize = int.MaxValue;
			}
			else
			{
				this.maxMessageSize = (int)transportConfiguration.MaxSendSize.Value.ToBytes();
			}
			MailTipsConfiguration.GetMailTipsConfigurationTracer.TraceDebug<int>((long)this.traceId, "Organization's max message size is ", this.maxMessageSize);
			Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)sender[ADRecipientSchema.MaxSendSize];
			if (!unlimited.IsUnlimited)
			{
				this.maxMessageSize = (int)unlimited.Value.ToBytes();
				MailTipsConfiguration.GetMailTipsConfigurationTracer.TraceDebug<int>((long)this.traceId, "Recipient's max message size is ", this.maxMessageSize);
			}
		}

		public const int MaxRecipientsPerGetMailTipsCall = 50;

		public const int MailTipsLargeAudienceCap = 1000;

		private static readonly Trace GetMailTipsConfigurationTracer = ExTraceGlobals.GetMailTipsConfigurationTracer;

		private int traceId;

		private int maxMessageSize;

		private int largeAudienceThreshold;

		private bool showExternalRecipientCount;

		private bool policyTipsEnabled;
	}
}
