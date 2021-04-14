using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class TransientDatabaseException : AITransientException
	{
		public TransientDatabaseException(LocalizedString explain, Exception innerException, RetrySchedule retrySchedule) : base(explain, innerException, retrySchedule)
		{
		}

		public TransientDatabaseException(Exception innerException) : this(LocalizedString.Empty, innerException, null)
		{
		}

		public TransientDatabaseException(LocalizedString explain) : this(explain, null, null)
		{
		}

		public TransientDatabaseException(RetrySchedule retrySchedule) : this(LocalizedString.Empty, null, retrySchedule)
		{
		}

		public TransientDatabaseException() : this(LocalizedString.Empty, null, null)
		{
		}
	}
}
