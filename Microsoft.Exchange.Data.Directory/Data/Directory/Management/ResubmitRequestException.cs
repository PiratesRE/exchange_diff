using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	internal class ResubmitRequestException : Exception
	{
		public ResubmitRequestException(ResubmitRequestResponseCode code, string message) : base(message)
		{
			this.errorCode = code;
		}

		public ResubmitRequestResponseCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private ResubmitRequestResponseCode errorCode;
	}
}
