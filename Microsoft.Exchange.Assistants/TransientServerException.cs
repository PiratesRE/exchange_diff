using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class TransientServerException : AITransientException
	{
		public TransientServerException(LocalizedString explain, Exception innerException, RetrySchedule retrySchedule) : base(explain, innerException, retrySchedule)
		{
		}

		public TransientServerException(Exception innerException) : this(LocalizedString.Empty, innerException, null)
		{
		}

		public TransientServerException(LocalizedString explain) : this(explain, null, null)
		{
		}

		public TransientServerException(RetrySchedule retrySchedule) : this(LocalizedString.Empty, null, retrySchedule)
		{
		}

		public TransientServerException() : this(LocalizedString.Empty, null, null)
		{
		}
	}
}
