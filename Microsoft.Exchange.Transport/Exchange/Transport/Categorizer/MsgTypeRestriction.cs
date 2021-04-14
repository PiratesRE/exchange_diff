using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class MsgTypeRestriction
	{
		public static void ExternalRecipientMessageTypeCheck(Expansion expansion, MailRecipient recipient)
		{
			bool flag = false;
			string sourceContext = string.Empty;
			if (expansion.Message.Type != ResolverMessageType.DR && expansion.Message.Type != ResolverMessageType.NDR && expansion.Message.Type != ResolverMessageType.MeetingForwardNotification && expansion.Message.Type != ResolverMessageType.AutoReply && !expansion.Message.AutoForwarded)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<int>(0L, "MsgType = {0}, No MsgType Restriction", (int)expansion.Message.Type);
				return;
			}
			RemoteDomainEntry domainContentConfig = ContentConverter.GetDomainContentConfig(recipient.Email, expansion.MailItem.OrganizationId);
			ExTraceGlobals.ResolverTracer.TraceDebug<SmtpDomainWithSubdomains>(0L, "domainContentConfig.DomainName = {0}", (domainContentConfig != null) ? domainContentConfig.DomainName : null);
			if (domainContentConfig != null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block DR = {0}", !domainContentConfig.DeliveryReportEnabled);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block NDR = {0}", !domainContentConfig.NDREnabled);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block MFN = {0}", !domainContentConfig.MFNEnabled);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block AutoReply = {0}", !domainContentConfig.AutoReplyEnabled);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block AutoForward = {0}", !domainContentConfig.AutoForwardEnabled);
				if (expansion.Message.Type == ResolverMessageType.DR && !domainContentConfig.DeliveryReportEnabled)
				{
					sourceContext = "BlockDRToExternalUser";
					recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockDRToExternalUser);
					flag = true;
				}
				else if (expansion.Message.Type == ResolverMessageType.NDR && !domainContentConfig.NDREnabled)
				{
					sourceContext = "BlockNDRToExternalUser";
					recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockNDRToExternalUser);
					flag = true;
				}
				else if (expansion.Message.Type == ResolverMessageType.MeetingForwardNotification && !domainContentConfig.MFNEnabled)
				{
					sourceContext = "BlockMFNToExternalUser";
					recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockMFNToExternalUser);
					flag = true;
				}
				else if (expansion.Message.Type == ResolverMessageType.AutoReply && !domainContentConfig.AutoReplyEnabled)
				{
					sourceContext = "BlockARToExternalUser";
					recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockARToExternalUser);
					flag = true;
				}
				else if (expansion.Message.AutoForwarded && !domainContentConfig.AutoForwardEnabled)
				{
					sourceContext = "BlockAFToExternalUser";
					recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockAFToExternalUser);
					flag = true;
				}
			}
			if (flag)
			{
				LatencyFormatter latencyFormatter = new LatencyFormatter(expansion.MailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
				MessageTrackingLog.TrackRecipientDrop(MessageTrackingSource.ROUTING, expansion.MailItem, recipient.Email, recipient.SmtpResponse, sourceContext, latencyFormatter);
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "blocked = {0}", flag);
		}
	}
}
