using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.MessageDepot;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport
{
	internal class SchedulableMailItem : ISchedulableMessage
	{
		public SchedulableMailItem(TransportMessageId transportMessageId, MessageEnvelope messageEnvelope, IEnumerable<IMessageScope> scopes, DateTime submitTime)
		{
			ArgumentValidator.ThrowIfNull("transportMessageId", transportMessageId);
			ArgumentValidator.ThrowIfNull("messageEnvelope", messageEnvelope);
			ArgumentValidator.ThrowIfNull("scopes", scopes);
			this.transportMessageId = transportMessageId;
			this.messageEnvelope = messageEnvelope;
			this.scopes = scopes;
			this.submitTime = submitTime;
		}

		public TransportMessageId Id
		{
			get
			{
				return this.transportMessageId;
			}
		}

		public DateTime SubmitTime
		{
			get
			{
				return this.submitTime;
			}
		}

		public IEnumerable<IMessageScope> Scopes
		{
			get
			{
				return this.scopes;
			}
		}

		public MessageEnvelope MessageEnvelope
		{
			get
			{
				return this.messageEnvelope;
			}
		}

		private readonly TransportMessageId transportMessageId;

		private readonly MessageEnvelope messageEnvelope;

		private readonly IEnumerable<IMessageScope> scopes;

		private readonly DateTime submitTime;
	}
}
