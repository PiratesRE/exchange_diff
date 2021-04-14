using System;

namespace Microsoft.Exchange.EdgeSync
{
	internal class EdgeSyncCycleFailedException : Exception
	{
		public EdgeSyncCycleFailedException(string message) : base(message)
		{
		}

		public EdgeSyncCycleFailedException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		public EdgeSyncCycleFailedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
