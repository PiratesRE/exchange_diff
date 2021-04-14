using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal static class MapiUtil
	{
		internal static TReturnValue TranslateMapiExceptionsWithReturnValue<TReturnValue>(IDiagnosticsSession tracer, LocalizedString errorString, Func<TReturnValue> mapiCall)
		{
			TReturnValue result = default(TReturnValue);
			MapiUtil.TranslateMapiExceptions(tracer, errorString, delegate
			{
				result = mapiCall();
			});
			return result;
		}

		internal static void TranslateMapiExceptions(IDiagnosticsSession tracer, LocalizedString errorString, Action mapiCall)
		{
			try
			{
				mapiCall();
			}
			catch (MapiRetryableException ex)
			{
				tracer.TraceError<LocalizedString, MapiRetryableException>("Got exception from MAPI: {0}, {1}.", errorString, ex);
				throw new OperationFailedException(errorString, ex);
			}
			catch (MapiPermanentException ex2)
			{
				tracer.TraceError<LocalizedString, MapiPermanentException>("Got exception from MAPI: {0}, {1}.", errorString, ex2);
				throw new ComponentFailedPermanentException(errorString, ex2);
			}
		}
	}
}
