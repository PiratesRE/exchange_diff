using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	public static class AsyncExceptionWrapperHelper
	{
		public static Exception GetRootException(Exception ex)
		{
			Exception ex2 = ex;
			while (ex2 is AsyncExceptionWrapper || ex2 is AsyncLocalizedExceptionWrapper)
			{
				ex2 = ex2.InnerException;
			}
			return ex2;
		}

		public static Exception GetAsyncWrapper(Exception ex)
		{
			if (ex is LocalizedException)
			{
				return new AsyncLocalizedExceptionWrapper(ex);
			}
			return new AsyncExceptionWrapper(ex);
		}
	}
}
