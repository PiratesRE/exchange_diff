using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class MissingObjectException : AIPermanentException
	{
		public MissingObjectException(LocalizedString message) : this(message, null)
		{
		}

		public MissingObjectException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
