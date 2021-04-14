using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NspiCallResult : RpcCallResult
	{
		public NspiCallResult(RpcException exception) : base(exception, ErrorCode.None, null)
		{
		}

		public NspiCallResult(NspiDataException exception) : base(null, ErrorCode.None, null)
		{
			this.nspiException = exception;
		}

		public NspiCallResult(NspiStatus nspiStatus) : base(null, (ErrorCode)nspiStatus, null)
		{
		}

		private NspiCallResult() : base(null, ErrorCode.None, null)
		{
		}

		public override bool IsSuccessful
		{
			get
			{
				return base.IsSuccessful && this.nspiException == null;
			}
		}

		public NspiDataException NspiException
		{
			get
			{
				return this.nspiException;
			}
		}

		public static NspiCallResult CreateSuccessfulResult()
		{
			return NspiCallResult.successResult;
		}

		private static readonly NspiCallResult successResult = new NspiCallResult();

		private readonly NspiDataException nspiException;
	}
}
