using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConnectCallResult : EmsmdbCallResult
	{
		public ConnectCallResult(Exception exception) : this(exception, null)
		{
		}

		public ConnectCallResult(Exception exception, IPropertyBag httpResponseInformation) : base(exception, httpResponseInformation)
		{
			this.serverVersion = null;
		}

		public ConnectCallResult(ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace, MapiVersion? serverVersion) : this(errorCode, remoteExceptionTrace, null, serverVersion, null)
		{
		}

		public ConnectCallResult(ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace, MonitoringActivityAuxiliaryBlock activityContext, MapiVersion? serverVersion, IPropertyBag httpResponseInformation) : base(null, errorCode, remoteExceptionTrace, activityContext, httpResponseInformation)
		{
			this.serverVersion = serverVersion;
		}

		private ConnectCallResult() : base(null, ErrorCode.None, null)
		{
		}

		public MapiVersion? ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public static ConnectCallResult CreateSuccessfulResult()
		{
			return ConnectCallResult.successResult;
		}

		private static readonly ConnectCallResult successResult = new ConnectCallResult();

		private readonly MapiVersion? serverVersion;
	}
}
