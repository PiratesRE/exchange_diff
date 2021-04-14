using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Agent.SystemProbeDrop
{
	internal class SystemProbeDropHelper
	{
		public static bool ShouldDropMessage(HeaderList headerList, string currentDropLocation)
		{
			ArgumentValidator.ThrowIfNull("headerList", headerList);
			ArgumentValidator.ThrowIfNullOrEmpty("currentDropLocation", currentDropLocation);
			if (headerList.FindFirst("X-LAMNotificationId") == null)
			{
				return false;
			}
			Header header = headerList.FindFirst("X-Exchange-System-Probe-Drop");
			return header != null && string.Equals(currentDropLocation, header.Value, StringComparison.OrdinalIgnoreCase);
		}

		public static void DiscardMessage(MailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			while (mailItem.Recipients.Count != 0)
			{
				EnvelopeRecipient recipient = mailItem.Recipients[0];
				mailItem.Recipients.Remove(recipient, DsnType.Expanded, SmtpResponse.ProbeMessageDropped);
			}
		}

		public static bool IsAgentEnabled()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Transport.SystemProbeDropAgent.Enabled;
		}

		public static bool CheckMailItemHeaders(MailItem mailitem)
		{
			return mailitem != null && mailitem.MimeDocument != null && mailitem.MimeDocument.RootPart != null && mailitem.MimeDocument.RootPart.Headers != null;
		}
	}
}
