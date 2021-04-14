using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RopHandlerHelper
	{
		internal static RopResult CallHandler(IRopHandler ropHandler, Func<RopResult> codeCallingIntoXso, IResultFactory resultFactory, Func<IResultFactory, ErrorCode, Exception, RopResult> createFailedResult)
		{
			return RopHandlerHelper.CallHandler<RopResult>(codeCallingIntoXso, (RopResult ropResult) => ropResult.ErrorCode, resultFactory, createFailedResult, (RopResult ropResult) => ropResult.RopId);
		}

		internal static T CallHandler<T>(NotificationQueue notificationQueue, Func<T> codeCallingIntoXso)
		{
			return RopHandlerHelper.CallHandler<T>(codeCallingIntoXso, (T result) => ErrorCode.None, null, (IResultFactory factory, ErrorCode errorCode, Exception exception) => default(T), (T ropResultOrNull) => RopId.Notify);
		}

		internal static RopResult CreateAndTraceFailedRopResult(IResultFactory resultFactory, Func<IResultFactory, ErrorCode, Exception, RopResult> createFailedResult, Exception exception, ErrorCode errorCode)
		{
			RopResult ropResult = createFailedResult(resultFactory, errorCode, exception);
			RopHandlerHelper.TraceRopResult(ropResult.RopId, exception, errorCode);
			return ropResult;
		}

		internal static void TraceRopResult(RopId ropId, Exception exception, ErrorCode errorCode)
		{
			if (errorCode != ErrorCode.None)
			{
				ProtocolLog.LogRopFailure(ropId, ExceptionTranslator.IsWarningErrorCode(errorCode), ExceptionTranslator.IsInterestingForProtocolLogging(ropId, errorCode), errorCode, exception);
			}
		}

		private static TResult CallHandler<TResult>(Func<TResult> codeCallingIntoXso, Func<TResult, ErrorCode> errorCodeExtractor, IResultFactory resultFactory, Func<IResultFactory, ErrorCode, Exception, TResult> createFailedResult, Func<TResult, RopId> getRopIdForTracing)
		{
			TResult tresult;
			Exception ex;
			ErrorCode errorCode;
			if (!ExceptionTranslator.TryExecuteCatchAndTranslateExceptions<TResult>(codeCallingIntoXso, errorCodeExtractor, false, out tresult, out ex, out errorCode))
			{
				tresult = createFailedResult(resultFactory, errorCode, ex);
			}
			RopHandlerHelper.TraceRopResult(getRopIdForTracing(tresult), ex, errorCode);
			return tresult;
		}

		internal static readonly FastTransferCopyMessagesFlag FastTransferCopyMessagesClientOnlyFlags = (FastTransferCopyMessagesFlag)6;

		internal static readonly FastTransferCopyPropertiesFlag FastTransferCopyPropertiesClientOnlyFlags = (FastTransferCopyPropertiesFlag)30;

		internal static readonly FastTransferCopyFlag FastTransferCopyClientOnlyFlags = (FastTransferCopyFlag)14U;
	}
}
