using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class AIPermanentException : AIException
	{
		protected AIPermanentException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}

		protected AIPermanentException() : this(LocalizedString.Empty, null)
		{
		}
	}
}
