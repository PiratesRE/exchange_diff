using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RpcCallResult : CallResult
	{
		protected RpcCallResult(Exception exception, ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace) : this(exception, errorCode, remoteExceptionTrace, null)
		{
		}

		protected RpcCallResult(Exception exception, ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace, MonitoringActivityAuxiliaryBlock activityContext)
		{
			this.exception = exception;
			this.errorCode = errorCode;
			this.remoteExceptionTrace = remoteExceptionTrace;
			this.activityContext = activityContext;
			RpcException ex = this.exception as RpcException;
			ProtocolException ex2 = this.exception as ProtocolException;
			if (ex != null)
			{
				this.statusCode = ex.ErrorCode;
				return;
			}
			if (ex2 == null)
			{
				this.statusCode = 0;
				return;
			}
			if (ex2 is ServiceTooBusyException)
			{
				this.statusCode = 1723;
				return;
			}
			if (ex2 is ServiceUnavailableException)
			{
				this.statusCode = 1722;
				return;
			}
			this.statusCode = 1726;
		}

		public override bool IsSuccessful
		{
			get
			{
				return this.StatusCode == 0 && this.errorCode == ErrorCode.None;
			}
		}

		public int StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public ErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public string RemoteExceptionTrace
		{
			get
			{
				if (this.remoteExceptionTrace == null)
				{
					return string.Empty;
				}
				return this.remoteExceptionTrace.ExceptionTrace;
			}
		}

		public string ActivityContext
		{
			get
			{
				if (this.activityContext != null)
				{
					return this.activityContext.ActivityContent;
				}
				return null;
			}
		}

		private const int RpcSServerUnavailable = 1722;

		private const int RpcSServerTooBusy = 1723;

		private const int RpcSCallFailed = 1726;

		private readonly Exception exception;

		private readonly ErrorCode errorCode;

		private readonly ExceptionTraceAuxiliaryBlock remoteExceptionTrace;

		private readonly MonitoringActivityAuxiliaryBlock activityContext;

		private readonly int statusCode;
	}
}
