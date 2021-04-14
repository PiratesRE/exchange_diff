using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class EnvelopeRecipientCollectionWrapper : EnvelopeRecipientCollection
	{
		internal EnvelopeRecipientCollectionWrapper(ReadOnlyEnvelopeRecipientCollection recipients)
		{
			this.recipients = recipients;
		}

		public override int Count
		{
			get
			{
				return this.recipients.Count;
			}
		}

		public override bool CanAdd
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override EnvelopeRecipient this[int index]
		{
			get
			{
				return this.recipients[index];
			}
		}

		public override bool Contains(RoutingAddress address)
		{
			return this.recipients.Contains(address);
		}

		public override EnvelopeRecipientCollection.Enumerator GetEnumerator()
		{
			return this.recipients.GetEnumerator();
		}

		public override void Add(RoutingAddress address)
		{
			throw new NotImplementedException();
		}

		public override void Clear()
		{
			throw new NotImplementedException();
		}

		public override bool Remove(EnvelopeRecipient recipient)
		{
			throw new NotImplementedException();
		}

		public override int Remove(RoutingAddress address)
		{
			throw new NotImplementedException();
		}

		public override bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse)
		{
			throw new NotImplementedException();
		}

		public override bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse, string sourceContext)
		{
			throw new NotImplementedException();
		}

		private ReadOnlyEnvelopeRecipientCollection recipients;
	}
}
