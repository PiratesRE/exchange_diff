using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class EnvelopeRecipientCollection : ReadOnlyEnvelopeRecipientCollection
	{
		internal EnvelopeRecipientCollection()
		{
		}

		public abstract bool CanAdd { get; }

		public abstract void Add(RoutingAddress address);

		public abstract void Clear();

		public abstract bool Remove(EnvelopeRecipient recipient);

		public abstract int Remove(RoutingAddress address);

		public abstract bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse);

		public abstract bool Remove(EnvelopeRecipient recipient, DsnType dsnType, SmtpResponse smtpResponse, string sourceContext);

		public struct Enumerator : IEnumerator<EnvelopeRecipient>, IDisposable, IEnumerator
		{
			internal Enumerator(IEnumerable items, Converter<object, EnvelopeRecipient> converter)
			{
				this.items = items;
				this.enumerator = items.GetEnumerator();
				this.converter = converter;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.converter(this.enumerator.Current);
				}
			}

			public EnvelopeRecipient Current
			{
				get
				{
					return this.converter(this.enumerator.Current);
				}
			}

			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			public void Reset()
			{
				this.enumerator = this.items.GetEnumerator();
			}

			public void Dispose()
			{
			}

			private IEnumerable items;

			private IEnumerator enumerator;

			private Converter<object, EnvelopeRecipient> converter;
		}
	}
}
