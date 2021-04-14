using System;

namespace Microsoft.Filtering
{
	public class ClassificationEngineInvalidCustomConfigurationException : ResultsValidationException
	{
		public ClassificationEngineInvalidCustomConfigurationException(string message, FilteringResults results) : base(message, results)
		{
		}
	}
}
