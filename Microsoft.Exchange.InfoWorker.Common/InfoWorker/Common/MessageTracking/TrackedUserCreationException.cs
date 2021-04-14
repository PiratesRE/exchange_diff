using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackedUserCreationException : Exception
	{
		public TrackedUserCreationException(string format, params object[] args) : base(string.Format(format, args))
		{
		}

		public TrackedUserCreationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
