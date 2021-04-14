using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class ReadOnlyEnvelopeRecipientCollection : IEnumerable<EnvelopeRecipient>, IEnumerable
	{
		internal ReadOnlyEnvelopeRecipientCollection()
		{
		}

		public abstract int Count { get; }

		public abstract EnvelopeRecipient this[int index]
		{
			get;
		}

		public abstract bool Contains(RoutingAddress address);

		IEnumerator<EnvelopeRecipient> IEnumerable<EnvelopeRecipient>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public abstract EnvelopeRecipientCollection.Enumerator GetEnumerator();
	}
}
