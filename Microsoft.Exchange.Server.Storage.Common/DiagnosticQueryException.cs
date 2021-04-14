using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class DiagnosticQueryException : Exception
	{
		public DiagnosticQueryException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public DiagnosticQueryException(string message) : base(message)
		{
		}
	}
}
