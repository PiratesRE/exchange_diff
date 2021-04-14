using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal class DiagnosticQueryTranslatorException : DiagnosticQueryException
	{
		public DiagnosticQueryTranslatorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public DiagnosticQueryTranslatorException(string message) : base(message)
		{
		}
	}
}
