using System;

namespace Microsoft.Exchange.PopImap.Core
{
	public class ErrorDetail
	{
		public string ErrorType { get; set; }

		public string ErrorMessage { get; set; }

		public string StackTrace { get; set; }

		public string UserEmail { get; set; }
	}
}
