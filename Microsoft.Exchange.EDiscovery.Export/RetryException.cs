using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class RetryException : Exception
	{
		public RetryException(Exception innerException, bool resetRetryCounter = false) : base(string.Empty, innerException)
		{
			this.ResetRetryCounter = resetRetryCounter;
		}

		public bool ResetRetryCounter { get; private set; }
	}
}
