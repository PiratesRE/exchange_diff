using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class InternalDeliverMailItemEventSource : DeliverMailItemEventSource
	{
		public InternalDeliverMailItemEventSource(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, RoutedMailItemWrapper deliverableMailItem, ulong sessionId, NextHopConnection nextHopConnection, string remoteHost, DeliveryAgentConnection.Stats stats)
		{
			this.source = new InternalDeliveryAgentEventSource(mexSession, deliverableMailItem, sessionId, nextHopConnection, remoteHost, stats);
		}

		public InternalDeliveryAgentEventSource InternalEventSource
		{
			get
			{
				return this.source;
			}
		}

		public override void FailQueue(SmtpResponse smtpResponse)
		{
			this.source.FailQueue(smtpResponse);
		}

		public override void DeferQueue(SmtpResponse smtpResponse)
		{
			this.source.DeferQueue(smtpResponse);
		}

		public override void DeferQueue(SmtpResponse smtpResponse, TimeSpan interval)
		{
			this.source.DeferQueue(smtpResponse, interval);
		}

		public override void AckMailItemSuccess(SmtpResponse smtpResponse)
		{
			this.source.AckMailItemSuccess(smtpResponse);
		}

		public override void AckMailItemDefer(SmtpResponse smtpResponse)
		{
			this.source.AckMailItemDefer(smtpResponse);
		}

		public override void AckMailItemFail(SmtpResponse smtpResponse)
		{
			this.source.AckMailItemFail(smtpResponse);
		}

		public override void AckRecipientSuccess(EnvelopeRecipient recipient, SmtpResponse smtpResponse)
		{
			this.source.AckRecipientSuccess(recipient, smtpResponse);
		}

		public override void AckRecipientDefer(EnvelopeRecipient recipient, SmtpResponse smtpResponse)
		{
			this.source.AckRecipientDefer(recipient, smtpResponse);
		}

		public override void AckRecipientFail(EnvelopeRecipient recipient, SmtpResponse smtpResponse)
		{
			this.source.AckRecipientFail(recipient, smtpResponse);
		}

		internal override void AddDsnParameters(string key, object value)
		{
			this.source.AddDsnParameters(key, value);
		}

		internal override bool TryGetDsnParameters(string key, out object value)
		{
			return this.source.TryGetDsnParameters(key, out value);
		}

		internal override void AddDsnParameters(EnvelopeRecipient recipient, string key, object value)
		{
			this.source.AddDsnParameters(recipient, key, value);
		}

		internal override bool TryGetDsnParameters(EnvelopeRecipient recipient, string key, out object value)
		{
			return this.source.TryGetDsnParameters(recipient, key, out value);
		}

		public override void UnregisterConnection(SmtpResponse smtpResponse)
		{
			this.source.UnregisterConnection(smtpResponse);
		}

		private InternalDeliveryAgentEventSource source;
	}
}
