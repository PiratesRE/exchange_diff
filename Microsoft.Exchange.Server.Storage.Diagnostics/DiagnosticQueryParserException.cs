using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal class DiagnosticQueryParserException : DiagnosticQueryException
	{
		public DiagnosticQueryParserException(string message) : base(message)
		{
		}

		public DiagnosticQueryParserException(string message, string query) : base(message)
		{
			this.query = query;
		}

		public DiagnosticQueryParserException(string message, string query, Exception innerException) : base(message, innerException)
		{
			this.query = query;
		}

		public string Query
		{
			get
			{
				return this.query;
			}
		}

		private string query;
	}
}
