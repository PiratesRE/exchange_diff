using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Smtp
{
	internal class SmtpMailItemSender
	{
		private SmtpMailItemSender()
		{
		}

		public static SmtpMailItemSender Instance
		{
			get
			{
				return SmtpMailItemSender.instance;
			}
		}

		private static string LocalFQDN
		{
			get
			{
				return SmtpMailItemSender.localFQDN;
			}
		}

		private static bool IsFrontendAndHubColocatedServer
		{
			get
			{
				return SmtpMailItemSender.isFrontendAndHubColocatedServer;
			}
		}

		public SmtpMailItemResult Send(IReadOnlyMailItem readOnlyMailItem)
		{
			return this.Send(readOnlyMailItem, false, TimeSpan.FromMinutes(15.0));
		}

		public SmtpMailItemResult Send(IReadOnlyMailItem readOnlyMailItem, bool useLocalHubOnly, TimeSpan waitTimeOut)
		{
			return this.Send(readOnlyMailItem, useLocalHubOnly, waitTimeOut, null);
		}

		public SmtpMailItemResult Send(IReadOnlyMailItem readOnlyMailItem, bool useLocalHubOnly, TimeSpan waitTimeOut, ISmtpMailItemSenderNotifications notificationHandler)
		{
			if (readOnlyMailItem == null)
			{
				throw new ArgumentNullException("readOnlyMailItem");
			}
			IEnumerable<INextHopServer> enumerable;
			if (useLocalHubOnly)
			{
				string text = SmtpMailItemSender.LocalFQDN;
				bool flag = SmtpMailItemSender.IsFrontendAndHubColocatedServer;
				if (string.IsNullOrEmpty(text))
				{
					throw new InvalidOperationException("Email is unable to be sent because the name of the local machine can not be detemined.");
				}
				enumerable = new List<INextHopServer>();
				((List<INextHopServer>)enumerable).Add(new NextHopFqdn(text, flag));
			}
			else
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionHubSelector, readOnlyMailItem.LatencyTracker);
				if (!Components.ProxyHubSelectorComponent.ProxyHubSelector.TrySelectHubServers(readOnlyMailItem, out enumerable))
				{
					throw new InvalidOperationException("Email is unable to be sent because Hub Selector didn't return any HUBs.");
				}
				LatencyTracker.EndTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionHubSelector, readOnlyMailItem.LatencyTracker);
			}
			NextHopSolutionKey key = new NextHopSolutionKey(NextHopType.Empty, "MailboxTransportSubmissionInternalProxy", Guid.Empty);
			SmtpMailItemResult smtpMailItemResult;
			using (SmtpMailItemNextHopConnection smtpMailItemNextHopConnection = new SmtpMailItemNextHopConnection(key, readOnlyMailItem, notificationHandler))
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtpOut, readOnlyMailItem.LatencyTracker);
				Components.SmtpOutConnectionHandler.HandleProxyConnection(smtpMailItemNextHopConnection, enumerable, true, null);
				smtpMailItemNextHopConnection.AckConnectionEvent.WaitOne(waitTimeOut);
				LatencyTracker.EndTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionSmtpOut, readOnlyMailItem.LatencyTracker);
				smtpMailItemResult = smtpMailItemNextHopConnection.SmtpMailItemResult;
			}
			return smtpMailItemResult;
		}

		private const string NextHopDomain = "MailboxTransportSubmissionInternalProxy";

		private static readonly SmtpMailItemSender instance = new SmtpMailItemSender();

		private static string localFQDN = Components.Configuration.LocalServer.TransportServer.Fqdn;

		private static bool isFrontendAndHubColocatedServer = Components.Configuration.LocalServer.TransportServer.IsHubTransportServer && Components.Configuration.LocalServer.TransportServer.IsFrontendTransportServer;
	}
}
