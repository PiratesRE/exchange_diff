using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class TransientMailboxException : AITransientException
	{
		public TransientMailboxException(LocalizedString explain, Exception innerException, RetrySchedule retrySchedule) : base(explain, innerException, retrySchedule)
		{
		}

		public TransientMailboxException(Exception innerException) : this(LocalizedString.Empty, innerException, null)
		{
		}

		public TransientMailboxException(LocalizedString explain) : this(explain, null, null)
		{
		}

		public TransientMailboxException(RetrySchedule retrySchedule) : this(LocalizedString.Empty, null, retrySchedule)
		{
		}

		public TransientMailboxException() : this(LocalizedString.Empty, null, null)
		{
		}
	}
}
