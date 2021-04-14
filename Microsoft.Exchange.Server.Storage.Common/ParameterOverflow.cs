using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ParameterOverflow : StoreException
	{
		public ParameterOverflow(LID lid, string message) : base(lid, ErrorCodeValue.ParameterOverflow, message)
		{
		}

		public ParameterOverflow(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.ParameterOverflow, message, innerException)
		{
		}
	}
}
