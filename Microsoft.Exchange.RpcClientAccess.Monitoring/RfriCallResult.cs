using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriCallResult : RpcCallResult
	{
		public RfriCallResult(RpcException exception) : base(exception, ErrorCode.None, null)
		{
		}

		public RfriCallResult(RfriStatus rfriStatus) : base(null, (ErrorCode)rfriStatus, null)
		{
		}

		private RfriCallResult() : base(null, ErrorCode.None, null)
		{
		}

		public static RfriCallResult CreateSuccessfulResult()
		{
			return RfriCallResult.successResult;
		}

		private static readonly RfriCallResult successResult = new RfriCallResult();
	}
}
