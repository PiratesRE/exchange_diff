using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal class BlindProxyNextHopConnection : NextHopConnection
	{
		public BlindProxyNextHopConnection(ProxySessionSetupHandler proxySetupHandler, NextHopSolutionKey key) : base(null, 0L, DeliveryPriority.Normal, null)
		{
			this.sessionSetupHandler = proxySetupHandler;
			this.key = key;
		}

		public override NextHopSolutionKey Key
		{
			get
			{
				return this.key;
			}
		}

		public override IReadOnlyMailItem ReadOnlyMailItem
		{
			get
			{
				throw new NotSupportedException("This should not be called in blind proxy mode");
			}
		}

		public override RoutedMailItem RoutedMailItem
		{
			get
			{
				throw new NotSupportedException("This should not be called in blind proxy mode");
			}
		}

		public override int MaxMessageRecipients
		{
			get
			{
				throw new NotSupportedException("This should not be called in blind proxy mode");
			}
		}

		public override int RecipientCount
		{
			get
			{
				throw new NotSupportedException("This should not be called in blind proxy mode");
			}
		}

		public override IList<MailRecipient> ReadyRecipientsList
		{
			get
			{
				throw new NotSupportedException("This should not be called in blind proxy mode");
			}
		}

		public override IEnumerable<MailRecipient> ReadyRecipients
		{
			get
			{
				throw new NotSupportedException("This should not be called in blind proxy mode");
			}
		}

		public override int ActiveQueueLength
		{
			get
			{
				return 0;
			}
		}

		public override int TotalQueueLength
		{
			get
			{
				return 0;
			}
		}

		public override void ConnectionAttemptSucceeded()
		{
		}

		public override RoutedMailItem GetNextRoutedMailItem()
		{
			throw new NotSupportedException("This should not be called in blind proxy mode");
		}

		public override IReadOnlyMailItem GetNextMailItem()
		{
			throw new NotSupportedException("This should not be called in blind proxy mode");
		}

		public override MailRecipient GetNextRecipient()
		{
			throw new NotSupportedException("This should not be called in blind proxy mode");
		}

		public override void NotifyConnectionFailedOver(string targetHostName, SmtpResponse failoverResponse, SessionSetupFailureReason failoverReason)
		{
			this.sessionSetupHandler.LogError(failoverReason, failoverResponse.ToString());
		}

		public override void AckConnection(MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckStatus status, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, bool resubmitWithoutHighAvailablityRouting, SessionSetupFailureReason failureReason)
		{
			switch (status)
			{
			case AckStatus.Pending:
			case AckStatus.Success:
			case AckStatus.Resubmit:
			case AckStatus.Skip:
				throw new InvalidOperationException("Invalid status for blind proxy");
			case AckStatus.Retry:
				if (!smtpResponse.Equals(SmtpResponse.Empty) && failureReason != SessionSetupFailureReason.None)
				{
					this.sessionSetupHandler.LogError(failureReason, smtpResponse.ToString());
				}
				this.sessionSetupHandler.HandleProxySessionDisconnection(smtpResponse, false, failureReason);
				return;
			case AckStatus.Fail:
				if (!smtpResponse.Equals(SmtpResponse.Empty) && failureReason != SessionSetupFailureReason.None)
				{
					this.sessionSetupHandler.LogError(failureReason, smtpResponse.ToString());
				}
				this.sessionSetupHandler.HandleProxySessionDisconnection(smtpResponse, true, failureReason);
				return;
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
			case AckStatus.Quarantine:
				return;
			default:
				return;
			}
		}

		public override void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails details, TimeSpan? retryInterval, MessageTrackingSource source, string messageTrackingSourceContext, LatencyComponent deliveryComponent, string remoteMta, bool shadowed, string primaryServer, bool reportEndToEndLatencies)
		{
			throw new NotSupportedException("This should not be called in blind proxy mode");
		}

		public override void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			throw new NotSupportedException("This should not be called in blind proxy mode");
		}

		public override void ResetQueueLastRetryTimeAndError()
		{
			throw new NotSupportedException("This should not be called in blind proxy mode");
		}

		private NextHopSolutionKey key;

		private ProxySessionSetupHandler sessionSetupHandler;
	}
}
