using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SingleInstanceItemHandlerTransientException : TransientException
	{
		public SingleInstanceItemHandlerTransientException(LocalizedString message) : base(message)
		{
		}

		public SingleInstanceItemHandlerTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
