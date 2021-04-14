using System;
using Microsoft.Filtering.Results;

namespace Microsoft.Filtering
{
	public class ResultsValidationException : FilteringException
	{
		public ResultsValidationException(string message, FilteringResults results) : base(string.Format("{0}\r\n{1}", message, ResultsFormatter.ConsoleFormatter.Format(results)), null)
		{
		}
	}
}
