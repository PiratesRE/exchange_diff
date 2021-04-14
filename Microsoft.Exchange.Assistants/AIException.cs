using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class AIException : LocalizedException
	{
		protected AIException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
		}
	}
}
