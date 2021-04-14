using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DummyCallResult : EmsmdbCallResult
	{
		public DummyCallResult(Exception exception) : base(exception, ErrorCode.None, null)
		{
		}

		public DummyCallResult(ErrorCode errorCode) : base(null, errorCode, null)
		{
		}

		private DummyCallResult() : base(null, ErrorCode.None, null)
		{
		}

		public static DummyCallResult CreateSuccessfulResult()
		{
			return DummyCallResult.successResult;
		}

		private static readonly DummyCallResult successResult = new DummyCallResult();
	}
}
