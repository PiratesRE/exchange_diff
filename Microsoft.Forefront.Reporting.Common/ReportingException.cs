using System;

namespace Microsoft.Forefront.Reporting.Common
{
	public class ReportingException : Exception
	{
		internal ReportingException()
		{
		}

		internal ReportingException(string message) : base(message)
		{
		}

		internal ReportingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
