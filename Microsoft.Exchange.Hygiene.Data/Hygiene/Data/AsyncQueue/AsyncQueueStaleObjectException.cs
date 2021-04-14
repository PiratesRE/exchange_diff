using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueStaleObjectException : Exception
	{
		public AsyncQueueStaleObjectException() : base("The AsyncQueue Request object has been modified in the background. Please refresh the object and save.")
		{
		}

		private const string ErrorMessage = "The AsyncQueue Request object has been modified in the background. Please refresh the object and save.";
	}
}
