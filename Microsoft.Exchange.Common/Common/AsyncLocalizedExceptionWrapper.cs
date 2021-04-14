using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	public class AsyncLocalizedExceptionWrapper : LocalizedException
	{
		public AsyncLocalizedExceptionWrapper(Exception innerException) : base(CommonStrings.AsyncExceptionMessage(innerException.Message), innerException)
		{
		}
	}
}
