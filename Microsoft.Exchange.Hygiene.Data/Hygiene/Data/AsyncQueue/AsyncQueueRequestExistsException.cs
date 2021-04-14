using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueRequestExistsException : Exception
	{
		public AsyncQueueRequestExistsException() : base("Request exists for the Tenant/Owner combination specified.")
		{
		}

		private const string ErrorMessage = "Request exists for the Tenant/Owner combination specified.";
	}
}
