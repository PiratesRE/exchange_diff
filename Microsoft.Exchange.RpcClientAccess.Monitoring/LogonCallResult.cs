using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LogonCallResult : EmsmdbCallResult
	{
		public LogonCallResult(Exception exception) : this(exception, null)
		{
		}

		public LogonCallResult(Exception exception, IPropertyBag httpResponseInformation) : base(exception, httpResponseInformation)
		{
			this.logonErrorCode = ErrorCode.None;
		}

		public LogonCallResult(ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace, ErrorCode logonErrorCode) : this(errorCode, remoteExceptionTrace, null, logonErrorCode, null)
		{
		}

		public LogonCallResult(ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace, MonitoringActivityAuxiliaryBlock activityContext, ErrorCode logonErrorCode, IPropertyBag httpResponseInformation) : base(null, errorCode, remoteExceptionTrace, activityContext, httpResponseInformation)
		{
			this.logonErrorCode = logonErrorCode;
		}

		private LogonCallResult() : base(null, ErrorCode.None, null)
		{
			this.logonErrorCode = ErrorCode.None;
		}

		public override bool IsSuccessful
		{
			get
			{
				return base.IsSuccessful && this.logonErrorCode == ErrorCode.None;
			}
		}

		public ErrorCode LogonErrorCode
		{
			get
			{
				return this.logonErrorCode;
			}
		}

		public static LogonCallResult CreateSuccessfulResult()
		{
			return LogonCallResult.successResult;
		}

		private static readonly LogonCallResult successResult = new LogonCallResult();

		private readonly ErrorCode logonErrorCode;
	}
}
