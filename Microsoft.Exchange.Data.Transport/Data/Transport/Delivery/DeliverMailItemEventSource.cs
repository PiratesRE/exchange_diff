using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	public abstract class DeliverMailItemEventSource
	{
		public abstract void FailQueue(SmtpResponse smtpResponse);

		public abstract void DeferQueue(SmtpResponse smtpResponse);

		public abstract void DeferQueue(SmtpResponse smtpResponse, TimeSpan interval);

		public abstract void AckMailItemSuccess(SmtpResponse smtpResponse);

		public abstract void AckMailItemDefer(SmtpResponse smtpResponse);

		public abstract void AckMailItemFail(SmtpResponse smtpResponse);

		public abstract void AckRecipientSuccess(EnvelopeRecipient recipient, SmtpResponse smtpResponse);

		public abstract void AckRecipientDefer(EnvelopeRecipient recipient, SmtpResponse smtpResponse);

		public abstract void AckRecipientFail(EnvelopeRecipient recipient, SmtpResponse smtpResponse);

		internal abstract void AddDsnParameters(string key, object value);

		internal abstract bool TryGetDsnParameters(string key, out object value);

		internal abstract void AddDsnParameters(EnvelopeRecipient recipient, string key, object value);

		internal abstract bool TryGetDsnParameters(EnvelopeRecipient recipient, string key, out object value);

		public abstract void UnregisterConnection(SmtpResponse smtpResponse);
	}
}
