using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class LamHelper
	{
		public static void PublishSuccessfulIrmDecryptionToLAM(MailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			if (mailItem.IsProbeMessage)
			{
				EventNotificationItem eventNotificationItem = LamHelper.CreateEventNotificationItem(mailItem);
				eventNotificationItem.AddCustomProperty("StateAttribute3", "IrmMessageSuccessfullyDecrypted");
				eventNotificationItem.Publish(false);
			}
		}

		public static void PublishSuccessfulIrmEncryptionToLAM(MailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			if (mailItem.IsProbeMessage)
			{
				EventNotificationItem eventNotificationItem = LamHelper.CreateEventNotificationItem(mailItem);
				eventNotificationItem.AddCustomProperty("StateAttribute3", "IrmMessageSuccessfullyEncrypted");
				eventNotificationItem.Publish(false);
			}
		}

		public static void PublishSuccessfulE4eDecryptionToLAM(MailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			if (mailItem.IsProbeMessage)
			{
				EventNotificationItem eventNotificationItem = LamHelper.CreateEventNotificationItem(mailItem);
				eventNotificationItem.AddCustomProperty("StateAttribute3", "E4eMessageSuccessfullyDecrypted");
				eventNotificationItem.Publish(false);
			}
		}

		public static void PublishSuccessfulE4eEncryptionToLAM(MailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			if (mailItem.IsProbeMessage)
			{
				EventNotificationItem eventNotificationItem = LamHelper.CreateEventNotificationItem(mailItem);
				eventNotificationItem.AddCustomProperty("StateAttribute3", "E4eMessageSuccessfullyEncrypted");
				eventNotificationItem.Publish(false);
			}
		}

		private static EventNotificationItem CreateEventNotificationItem(MailItem mailItem)
		{
			string text = string.Empty;
			TransportMailItem transportMailItem = TransportUtils.GetTransportMailItem(mailItem);
			if (transportMailItem != null)
			{
				text = transportMailItem.ProbeName;
				if (string.IsNullOrEmpty(text))
				{
					transportMailItem.UpdateCachedHeaders();
					text = transportMailItem.ProbeName;
				}
			}
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Transport.Name, ExchangeComponent.Rms.Name, text, ResultSeverityLevel.Verbose);
			eventNotificationItem.AddCustomProperty("StateAttribute1", mailItem.Message.MessageId);
			eventNotificationItem.AddCustomProperty("StateAttribute2", "AGENTINFO");
			eventNotificationItem.StateAttribute4 = mailItem.SystemProbeId.ToString();
			return eventNotificationItem;
		}

		private const string LamMessageIdAttributeName = "StateAttribute1";

		private const string LamTransportSmtpProbeResultTypeAttributeName = "StateAttribute2";

		private const string LamTransportSmtpProbeResultValueAttributeName = "StateAttribute3";

		private const string LamTransportSmtpProbeResultTypeValue = "AGENTINFO";

		private const string LamIrmTransportSmtpProbeSuccessfullyDecryptedValue = "IrmMessageSuccessfullyDecrypted";

		private const string LamIrmTransportSmtpProbeSuccessfullyEncryptedValue = "IrmMessageSuccessfullyEncrypted";

		private const string LamE4eTransportSmtpProbeSuccessfullyDecryptedValue = "E4eMessageSuccessfullyDecrypted";

		private const string LamE4eTransportSmtpProbeSuccessfullyEncryptedValue = "E4eMessageSuccessfullyEncrypted";
	}
}
