using System;

namespace Microsoft.Exchange.Data
{
	public class DependencyValidationError : ValidationError
	{
		public DependencyValidationError(string feature, bool featureValue, string dependencyFeatureName, bool dependencyFeatureValue) : base(DataStrings.DependencyCheckFailed(feature, featureValue.ToString(), dependencyFeatureName, dependencyFeatureValue.ToString()))
		{
		}
	}
}
