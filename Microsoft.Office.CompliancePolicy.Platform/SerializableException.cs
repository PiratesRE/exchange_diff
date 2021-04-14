using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	internal class SerializableException
	{
		public SerializableException(Exception ex)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			List<string> list;
			List<string> list2;
			string exceptionChain;
			ExecutionLog.GetExceptionTypeAndDetails(ex, out list, out list2, out exceptionChain, true);
			this.ExceptionChain = exceptionChain;
		}

		public string ExceptionChain { get; private set; }
	}
}
