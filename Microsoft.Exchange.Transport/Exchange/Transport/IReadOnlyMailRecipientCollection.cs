using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport
{
	internal interface IReadOnlyMailRecipientCollection : IEnumerable<MailRecipient>, IEnumerable
	{
		int Count { get; }

		MailRecipient this[int index]
		{
			get;
		}

		IEnumerable<MailRecipient> All { get; }

		IEnumerable<MailRecipient> AllUnprocessed { get; }

		bool Contains(MailRecipient item);
	}
}
