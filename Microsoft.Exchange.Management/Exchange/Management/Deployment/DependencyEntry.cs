using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class DependencyEntry
	{
		internal DependencyEntry(string featureName, string dependencyFeatureName, GetFeatureValue getFeatureValue, GetDependencyValue getDependencyValue, SetDependencyValue setDependencyValue)
		{
			if (featureName == null)
			{
				throw new ArgumentNullException("featureName");
			}
			if (dependencyFeatureName == null)
			{
				throw new ArgumentNullException("dependencyFeatureName");
			}
			if (getFeatureValue == null)
			{
				throw new ArgumentNullException("getFeatureValue");
			}
			if (getDependencyValue == null)
			{
				throw new ArgumentNullException("getDependencyValue");
			}
			if (setDependencyValue == null)
			{
				throw new ArgumentNullException("setDependencyValue");
			}
			this.FeatureName = featureName;
			this.DependencyFeatureName = dependencyFeatureName;
			this.GetFeatureValue = getFeatureValue;
			this.GetDependencyValue = getDependencyValue;
			this.SetDependencyValue = setDependencyValue;
		}

		internal readonly string FeatureName;

		internal readonly string DependencyFeatureName;

		internal readonly GetFeatureValue GetFeatureValue;

		internal readonly GetDependencyValue GetDependencyValue;

		internal readonly SetDependencyValue SetDependencyValue;
	}
}
