using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class IllegalOperationNewCookieWithDataException : Exception
	{
		public IllegalOperationNewCookieWithDataException() : base("Attempted to create a new cookie with a non-null data field.")
		{
		}

		private const string ErrorMessage = "Attempted to create a new cookie with a non-null data field.";
	}
}
