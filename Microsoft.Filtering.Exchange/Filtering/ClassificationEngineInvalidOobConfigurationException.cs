using System;

namespace Microsoft.Filtering
{
	public class ClassificationEngineInvalidOobConfigurationException : ResultsValidationException
	{
		public ClassificationEngineInvalidOobConfigurationException(string message, FilteringResults results) : base(message, results)
		{
		}
	}
}
