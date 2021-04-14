using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class IllegalOperationNoCallerSpecifiedException : Exception
	{
		public IllegalOperationNoCallerSpecifiedException() : base("No caller specified")
		{
		}

		private const string ErrorMessage = "No caller specified";
	}
}
