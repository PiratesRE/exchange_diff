using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class CommandParsingHelper
	{
		public static int GetNameValuePairSeparatorIndex(byte[] protocolCommand, Offset nameValuePairOffset, byte separator = 61)
		{
			ArgumentValidator.ThrowIfNull("protocolCommand", protocolCommand);
			ArgumentValidator.ThrowIfInvalidValue<Offset>("nameValuePairOffset", nameValuePairOffset, (Offset offset) => offset.Length > 0);
			ArgumentValidator.ThrowIfOutOfRange<int>("nameValuePairOffset.Start", nameValuePairOffset.Start, 0, protocolCommand.Length);
			return Array.IndexOf<byte>(protocolCommand, separator, nameValuePairOffset.Start, nameValuePairOffset.Length);
		}

		public static bool ShouldRejectMailItem(RoutingAddress fromAddress, SmtpInSessionState sessionState, bool checkRecipientCount, out SmtpResponse failureSmtpResponse)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			failureSmtpResponse = SmtpResponse.Empty;
			if (sessionState.ServerState.RejectSubmits)
			{
				failureSmtpResponse = sessionState.ServerState.RejectionSmtpResponse;
				return true;
			}
			if (sessionState.ServerState.RejectMailFromInternet && (fromAddress == RoutingAddress.NullReversePath || (!sessionState.Configuration.TransportConfiguration.FirstOrgAcceptedDomainTable.CheckInternal(SmtpDomain.GetDomainPart(fromAddress)) && !sessionState.Configuration.TransportConfiguration.SmtpAcceptAnyRecipient)))
			{
				failureSmtpResponse = sessionState.ServerState.RejectionSmtpResponse;
				return true;
			}
			if (checkRecipientCount && sessionState.TransportMailItem != null && sessionState.TransportMailItem.Recipients != null && sessionState.TransportMailItem.Recipients.Count <= 0)
			{
				failureSmtpResponse = SmtpResponse.MailboxDisabled;
				return true;
			}
			return false;
		}
	}
}
