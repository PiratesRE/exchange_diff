using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class OofRestriction
	{
		public static void InternalUserOofCheck(Expansion expansion, MailRecipient recipient)
		{
			bool flag = false;
			string sourceContext = string.Empty;
			if (expansion.MailItem.TransportSettings.OpenDomainRoutingEnabled)
			{
				if (expansion.Message.Type == ResolverMessageType.InternalOOF)
				{
					sourceContext = "BlockInternalOofToInternalOpenDomainUser";
					recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockInternalOofToInternalOpenDomainUser);
					flag = true;
				}
			}
			else if (expansion.Message.Type == ResolverMessageType.ExternalOOF && expansion.Sender.InternalOrgSender)
			{
				sourceContext = "BlockExternalOofToInternalUser";
				recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockExternalOofToInternalUser);
				flag = true;
			}
			if (flag)
			{
				LatencyFormatter latencyFormatter = new LatencyFormatter(expansion.MailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
				MessageTrackingLog.TrackRecipientDrop(MessageTrackingSource.ROUTING, expansion.MailItem, recipient.Email, recipient.SmtpResponse, sourceContext, latencyFormatter);
			}
		}

		public static void ExternalUserOofCheck(Expansion expansion, MailRecipient recipient)
		{
			bool flag = false;
			string sourceContext = string.Empty;
			switch (expansion.Message.Type)
			{
			case ResolverMessageType.LegacyOOF:
			case ResolverMessageType.InternalOOF:
			case ResolverMessageType.ExternalOOF:
			{
				bool allowExternalOofs = expansion.Sender.AllowExternalOofs;
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "UserAllowedExternalOofs = {0}", allowExternalOofs);
				RemoteDomainEntry domainContentConfig = ContentConverter.GetDomainContentConfig(recipient.Email, expansion.MailItem.OrganizationId);
				ExTraceGlobals.ResolverTracer.TraceDebug<SmtpDomainWithSubdomains>(0L, "domainContentConfig.DomainName = {0}", (domainContentConfig != null) ? domainContentConfig.DomainName : null);
				bool flag2;
				bool flag3;
				bool flag4;
				OofRestriction.GetOOFSettings(domainContentConfig, allowExternalOofs, out flag2, out flag3, out flag4);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block Internal OOF = {0}", flag2);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block External OOF = {0}", flag3);
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "Block Legacy OOF = {0}", flag4);
				switch (expansion.Message.Type)
				{
				case ResolverMessageType.LegacyOOF:
					if (flag4)
					{
						sourceContext = "BlockLegacyOofToExternalUser";
						recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockLegacyOofToExternalUser);
						flag = true;
					}
					break;
				case ResolverMessageType.InternalOOF:
					if (flag2)
					{
						sourceContext = "BlockInternalOofToExternalUser";
						recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockInternalOofToExternalUser);
						flag = true;
					}
					break;
				case ResolverMessageType.ExternalOOF:
					if (flag3)
					{
						sourceContext = "BlockExternalOofToExternalUser";
						recipient.Ack(AckStatus.SuccessNoDsn, AckReason.BlockExternalOofToExternalUser);
						flag = true;
					}
					break;
				}
				if (flag)
				{
					LatencyFormatter latencyFormatter = new LatencyFormatter(expansion.MailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
					MessageTrackingLog.TrackRecipientDrop(MessageTrackingSource.ROUTING, expansion.MailItem, recipient.Email, recipient.SmtpResponse, sourceContext, latencyFormatter);
				}
				ExTraceGlobals.ResolverTracer.TraceDebug<bool>(0L, "blocked = {0}", flag);
				return;
			}
			default:
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "No OOF Restriction");
				return;
			}
		}

		private static void GetOOFSettings(RemoteDomainEntry domainConfig, bool userAllowedExternalOofs, out bool blockInternalOof, out bool blockExternalOof, out bool blockLegacyOof)
		{
			AcceptMessageType acceptMessageType = AcceptMessageType.Default;
			if (domainConfig != null)
			{
				acceptMessageType = domainConfig.OofSettings;
			}
			bool flag = (acceptMessageType & AcceptMessageType.BlockOOF) != AcceptMessageType.Default;
			bool flag2 = (acceptMessageType & AcceptMessageType.InternalDomain) != AcceptMessageType.Default;
			bool flag3 = (acceptMessageType & AcceptMessageType.LegacyOOF) != AcceptMessageType.Default;
			blockInternalOof = (flag || !flag2);
			blockExternalOof = (flag || flag2 || !userAllowedExternalOofs);
			blockLegacyOof = (flag || (!flag2 && !userAllowedExternalOofs) || !flag3);
		}
	}
}
