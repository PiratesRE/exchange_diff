using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExceptionExtensionMethods
	{
		internal static T FindException<T>(this Exception exception) where T : Exception
		{
			if (exception == null)
			{
				return default(T);
			}
			T t = exception as T;
			if (t != null)
			{
				return t;
			}
			AggregateException ex = exception as AggregateException;
			if (ex != null && ex.InnerExceptions != null)
			{
				foreach (Exception exception2 in ex.InnerExceptions)
				{
					T t2 = exception2.FindException<T>();
					if (t2 != null)
					{
						return t2;
					}
				}
			}
			return default(T);
		}

		internal static bool TryGetFailureInformation(this Exception exception, out ResponseCode? responseCode, out LID? failureLID, out HttpStatusCode? httpStatusCode, out string httpStatusDescription, out string failureDescription)
		{
			responseCode = null;
			failureLID = null;
			httpStatusCode = null;
			httpStatusDescription = null;
			failureDescription = null;
			if (exception == null)
			{
				return false;
			}
			ProtocolFailureException ex = exception.FindException<ProtocolFailureException>();
			if (ex != null)
			{
				if (ex.ResponseCode != ResponseCode.Success)
				{
					responseCode = new ResponseCode?(ex.ResponseCode);
				}
				failureLID = new LID?(ex.FailureLID);
				httpStatusCode = new HttpStatusCode?(ex.HttpStatusCode);
				httpStatusDescription = ex.HttpStatusDescription;
				failureDescription = ex.FailureDescription;
			}
			else
			{
				responseCode = new ResponseCode?(ResponseCode.UnknownFailure);
				failureLID = new LID?((LID)53020);
				httpStatusCode = new HttpStatusCode?(HttpStatusCode.OK);
				httpStatusDescription = HttpStatusCode.OK.ToString();
				failureDescription = "Internal Failure";
			}
			return true;
		}
	}
}
