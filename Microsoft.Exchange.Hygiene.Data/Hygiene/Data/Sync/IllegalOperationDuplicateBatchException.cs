using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class IllegalOperationDuplicateBatchException : Exception
	{
		public IllegalOperationDuplicateBatchException() : base("Calling daemon specified a duplicate batch id")
		{
		}

		private const string ErrorMessage = "Calling daemon specified a duplicate batch id";
	}
}
