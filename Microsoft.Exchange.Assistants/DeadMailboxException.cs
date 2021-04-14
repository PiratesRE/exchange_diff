using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class DeadMailboxException : AIPermanentException
	{
		public DeadMailboxException(Exception innerException) : base(Strings.descDeadMailboxException, innerException)
		{
		}

		public DeadMailboxException(LocalizedString explain) : base(explain, null)
		{
		}

		public DeadMailboxException(LocalizedString explain, Exception innerException) : base(explain, innerException)
		{
		}
	}
}
