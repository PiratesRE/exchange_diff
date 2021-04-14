using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class IllegalOperationCallerNotCookieOwnerException : Exception
	{
		public IllegalOperationCallerNotCookieOwnerException() : base("Calling daemon is not the cookie owner")
		{
		}

		private const string ErrorMessage = "Calling daemon is not the cookie owner";
	}
}
