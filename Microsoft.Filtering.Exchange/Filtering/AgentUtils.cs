using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Filtering
{
	public static class AgentUtils
	{
		public static void NdrMessage(MailItem mailItem, SmtpResponse response)
		{
			TransportMailItem transportMailItem = AgentUtils.GetTransportMailItem(mailItem);
			transportMailItem.SuppressBodyInDsn = true;
			foreach (EnvelopeRecipient recipient in mailItem.Recipients)
			{
				mailItem.Recipients.Remove(recipient, DsnType.Failure, response);
			}
		}

		private static TransportMailItem GetTransportMailItem(MailItem mailItem)
		{
			return (TransportMailItem)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
		}
	}
}
