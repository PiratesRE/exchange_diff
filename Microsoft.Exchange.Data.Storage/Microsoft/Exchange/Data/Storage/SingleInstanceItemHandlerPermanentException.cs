using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SingleInstanceItemHandlerPermanentException : LocalizedException
	{
		public SingleInstanceItemHandlerPermanentException(LocalizedString message) : base(message)
		{
		}

		public SingleInstanceItemHandlerPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
