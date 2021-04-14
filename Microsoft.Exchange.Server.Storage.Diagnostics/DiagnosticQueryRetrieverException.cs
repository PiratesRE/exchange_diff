using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal class DiagnosticQueryRetrieverException : DiagnosticQueryException
	{
		public DiagnosticQueryRetrieverException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public DiagnosticQueryRetrieverException(string message) : base(message)
		{
		}
	}
}
