using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class DeliveryThrottlingAgent : SmtpReceiveAgent
	{
		internal DeliveryThrottlingAgent()
		{
			this.defaultConnectionWait = DeliveryConfiguration.Instance.Throttling.AcquireConnectionTimeout;
			base.OnConnect += this.ConnectHandler;
			base.OnEndOfData += this.EndOfDataEventHandler;
			base.OnXSessionParamsCommand += this.XSessionParamsCommandHandler;
			base.OnRcptCommand += this.RcptCommandHandler;
			base.OnDisconnect += this.DisconnectHandler;
			base.OnMailCommand += this.MailCommandHandler;
		}

		private void ConnectHandler(ConnectEventSource source, ConnectEventArgs args)
		{
			DeliveryThrottlingAgent.Diag.TraceDebug(0, (long)this.GetHashCode(), "OnConnectHandler started");
			MSExchangeStoreDriver.LocalDeliveryCalls.Increment();
			if (!DeliveryThrottling.Instance.CheckAndTrackThrottleServer(args.SmtpSession.SessionId))
			{
				MSExchangeStoreDriver.DeliveryRetry.Increment();
				source.RejectConnection(AckReason.MailboxServerThreadLimitExceeded);
			}
		}

		private void XSessionParamsCommandHandler(ReceiveCommandEventSource source, XSessionParamsCommandEventArgs args)
		{
			DeliveryThrottlingAgent.Diag.TraceDebug(0, (long)this.GetHashCode(), "XSessionParamsCommandHandler started");
			if (DeliveryConfiguration.Instance.Throttling.DynamicMailboxDatabaseThrottlingEnabled)
			{
				DeliveryThrottling.Instance.ResetSession(args.SmtpSession.SessionId);
				this.destinationMdbGuid = args.DestinationMdbGuid;
				this.connectionManager = DeliveryThrottling.Instance.MailboxDatabaseCollectionManager.GetConnectionManager(args.DestinationMdbGuid);
				if (!DeliveryThrottling.Instance.CheckAndTrackDynamicThrottleMDBPendingConnections(this.destinationMdbGuid, this.connectionManager, args.SmtpSession.SessionId, args.SmtpSession.RemoteEndPoint.Address, out this.mdbHealthMonitors))
				{
					MSExchangeStoreDriver.DeliveryRetry.Increment();
					source.RejectCommand(AckReason.DynamicMailboxDatabaseThrottlingLimitExceeded);
					return;
				}
			}
			else if (!DeliveryThrottling.Instance.CheckAndTrackThrottleMDB(args.DestinationMdbGuid, args.SmtpSession.SessionId, out this.mdbHealthMonitors))
			{
				MSExchangeStoreDriver.DeliveryRetry.Increment();
				source.RejectCommand(AckReason.MailboxDatabaseThreadLimitExceeded);
			}
		}

		private void MailCommandHandler(ReceiveCommandEventSource source, MailCommandEventArgs args)
		{
			DeliveryThrottling.Instance.SetSessionMessageSize(args.Size, args.SmtpSession.SessionId);
			if (!DeliveryConfiguration.Instance.Throttling.DynamicMailboxDatabaseThrottlingEnabled)
			{
				return;
			}
			if (this.destinationMdbGuid == Guid.Empty || this.connectionManager == null)
			{
				DeliveryThrottlingAgent.Diag.TraceDebug<long>(0, (long)this.GetHashCode(), "Dynamic Throttling: No destination mailbox database specified for this connection, no throttling applied. SessionId {0}", args.SmtpSession.SessionId);
				return;
			}
			if (this.mailboxDatabaseConnectionInfo != null)
			{
				DeliveryThrottlingAgent.Diag.TraceDebug<long>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection was previously acquired. Being released before attempt to reacquire. SessionId {0}", args.SmtpSession.SessionId);
				this.connectionManager.Release(ref this.mailboxDatabaseConnectionInfo);
			}
			if (!DeliveryThrottling.Instance.CheckAndTrackDynamicThrottleMDBTimeout(this.destinationMdbGuid, this.mailboxDatabaseConnectionInfo, this.connectionManager, args.SmtpSession.SessionId, args.SmtpSession.RemoteEndPoint.Address, this.defaultConnectionWait, this.mdbHealthMonitors))
			{
				DeliveryThrottling.Instance.DecrementCurrentMessageSize(args.SmtpSession.SessionId);
				source.RejectCommand(AckReason.DynamicMailboxDatabaseThrottlingLimitExceeded);
			}
			this.connectionManager.UpdateLastActivityTime(args.SmtpSession.SessionId);
		}

		private void RcptCommandHandler(ReceiveCommandEventSource source, RcptCommandEventArgs args)
		{
			DeliveryThrottlingAgent.Diag.TraceDebug(0, (long)this.GetHashCode(), "RcptCommandHandler started");
			if (!DeliveryThrottling.Instance.CheckAndTrackThrottleRecipient(args.RecipientAddress, args.SmtpSession.SessionId, this.destinationMdbGuid.ToString(), args.MailItem.TenantId))
			{
				MSExchangeStoreDriver.DeliveryRetry.Increment();
				source.RejectCommand(AckReason.RecipientThreadLimitExceeded);
			}
			if (this.connectionManager != null)
			{
				this.connectionManager.UpdateLastActivityTime(args.SmtpSession.SessionId);
			}
		}

		private void DisconnectHandler(DisconnectEventSource source, DisconnectEventArgs args)
		{
			DeliveryThrottlingAgent.Diag.TraceDebug(0, (long)this.GetHashCode(), "DisconnectHandler started");
			DeliveryThrottling.Instance.ClearSession(args.SmtpSession.SessionId);
			if (DeliveryConfiguration.Instance.Throttling.DynamicMailboxDatabaseThrottlingEnabled && this.connectionManager != null)
			{
				if (this.mailboxDatabaseConnectionInfo != null)
				{
					DeliveryThrottlingAgent.Diag.TraceDebug<long>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection lock was previously acquired. Being released during disconnect. SessionId {0}", args.SmtpSession.SessionId);
					if (!this.connectionManager.Release(ref this.mailboxDatabaseConnectionInfo))
					{
						DeliveryThrottlingAgent.Diag.TraceWarning<long>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection lock was previously acquired but release returned false during disconnect. SessionId {0}", args.SmtpSession.SessionId);
					}
				}
				bool flag = this.connectionManager.RemoveConnection(args.SmtpSession.SessionId, args.SmtpSession.RemoteEndPoint.Address);
				this.connectionManager = null;
				if (flag)
				{
					DeliveryThrottlingAgent.Diag.TraceDebug<Guid, long, IPAddress>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection removed. MDB {0} SessionId {1} IP {2}", this.destinationMdbGuid, args.SmtpSession.SessionId, args.SmtpSession.RemoteEndPoint.Address);
					return;
				}
				DeliveryThrottlingAgent.Diag.TraceWarning<Guid, long, IPAddress>(0, (long)this.GetHashCode(), "Dynamic Throttling: Remove Connection returned false during disconnect. MDB {0} SessionId {1} IP {2}", this.destinationMdbGuid, args.SmtpSession.SessionId, args.SmtpSession.RemoteEndPoint.Address);
			}
		}

		private void EndOfDataEventHandler(ReceiveMessageEventSource source, EndOfDataEventArgs args)
		{
			if (!DeliveryThrottling.Instance.CheckAndTrackThrottleConcurrentMessageSizeLimit(args.SmtpSession.SessionId, args.MailItem.Recipients.Count))
			{
				MSExchangeStoreDriver.DeliveryRetry.Increment();
				source.RejectMessage(AckReason.MaxConcurrentMessageSizeLimitExceeded);
				return;
			}
			string internetMessageId = args.MailItem.InternetMessageId;
			if (string.IsNullOrEmpty(internetMessageId))
			{
				DeliveryThrottlingAgent.Diag.TraceWarning(0, (long)this.GetHashCode(), "MessageId header is missing. Poison handling is disabled");
				return;
			}
			int crashCount = 0;
			if (DeliveryConfiguration.Instance.PoisonHandler.VerifyPoisonMessage(internetMessageId, out crashCount))
			{
				DeliveryThrottlingAgent.Diag.TraceError<string>(0, (long)this.GetHashCode(), "Poison message identified. Message ID: {0}", internetMessageId);
				source.RejectMessage(AckReason.InboundPoisonMessage(crashCount));
				return;
			}
			PoisonHandler<DeliveryPoisonContext>.Context = new DeliveryPoisonContext(internetMessageId);
		}

		private static readonly Trace Diag = ExTraceGlobals.StoreDriverDeliveryTracer;

		private readonly TimeSpan defaultConnectionWait;

		private Guid destinationMdbGuid;

		private List<KeyValuePair<string, double>> mdbHealthMonitors;

		private IMailboxDatabaseConnectionManager connectionManager;

		private IMailboxDatabaseConnectionInfo mailboxDatabaseConnectionInfo;
	}
}
