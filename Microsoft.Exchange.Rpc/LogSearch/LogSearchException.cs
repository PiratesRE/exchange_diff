using System;

namespace Microsoft.Exchange.Rpc.LogSearch
{
	internal class LogSearchException : Exception
	{
		public LogSearchException(int result)
		{
			this.errorCode = result;
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private int errorCode;
	}
}
