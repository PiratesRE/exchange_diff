using System;

namespace Microsoft.Exchange.Net.Mserve
{
	internal sealed class FailedAddress
	{
		public FailedAddress(string name, int errorCode, bool isTransientError)
		{
			this.name = name;
			this.errorCode = errorCode;
			this.isTransientError = isTransientError;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public bool IsTransientError
		{
			get
			{
				return this.isTransientError;
			}
		}

		private readonly string name;

		private readonly int errorCode;

		private readonly bool isTransientError;
	}
}
