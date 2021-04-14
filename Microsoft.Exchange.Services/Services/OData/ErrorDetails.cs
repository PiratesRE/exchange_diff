using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OData
{
	public class ErrorDetails
	{
		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public Exception Exception { get; set; }

		public Dictionary<string, string> AdditionalProperties { get; set; }
	}
}
