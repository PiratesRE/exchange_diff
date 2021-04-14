using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationMailboxTransportComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationMailboxTransportComponent() : base("MailboxTransport")
		{
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "ParkedMeetingMessagesRetentionPeriod", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "MailboxTransportSmtpIn", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "DeliveryHangRecovery", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "InferenceClassificationAgent", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "UseParticipantSmtpEmailAddress", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "CheckArbitrationMailboxCapacity", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "ProcessSeriesMeetingMessages", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "UseFopeReceivedSpfHeader", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxTransport.settings.ini", "OrderSeriesMeetingMessages", typeof(IFeature), false));
		}

		public VariantConfigurationSection ParkedMeetingMessagesRetentionPeriod
		{
			get
			{
				return base["ParkedMeetingMessagesRetentionPeriod"];
			}
		}

		public VariantConfigurationSection MailboxTransportSmtpIn
		{
			get
			{
				return base["MailboxTransportSmtpIn"];
			}
		}

		public VariantConfigurationSection DeliveryHangRecovery
		{
			get
			{
				return base["DeliveryHangRecovery"];
			}
		}

		public VariantConfigurationSection InferenceClassificationAgent
		{
			get
			{
				return base["InferenceClassificationAgent"];
			}
		}

		public VariantConfigurationSection UseParticipantSmtpEmailAddress
		{
			get
			{
				return base["UseParticipantSmtpEmailAddress"];
			}
		}

		public VariantConfigurationSection CheckArbitrationMailboxCapacity
		{
			get
			{
				return base["CheckArbitrationMailboxCapacity"];
			}
		}

		public VariantConfigurationSection ProcessSeriesMeetingMessages
		{
			get
			{
				return base["ProcessSeriesMeetingMessages"];
			}
		}

		public VariantConfigurationSection UseFopeReceivedSpfHeader
		{
			get
			{
				return base["UseFopeReceivedSpfHeader"];
			}
		}

		public VariantConfigurationSection OrderSeriesMeetingMessages
		{
			get
			{
				return base["OrderSeriesMeetingMessages"];
			}
		}
	}
}
