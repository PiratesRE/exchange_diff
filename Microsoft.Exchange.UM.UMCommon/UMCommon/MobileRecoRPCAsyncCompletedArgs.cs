using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MobileRecoRPCAsyncCompletedArgs
	{
		public string Result { get; private set; }

		public int ErrorCode { get; private set; }

		public string ErrorMessage { get; private set; }

		public MobileRecoRPCAsyncCompletedArgs(string result, int errorCode, string errorMessage)
		{
			this.Result = result;
			this.ErrorCode = errorCode;
			this.ErrorMessage = errorMessage;
		}
	}
}
