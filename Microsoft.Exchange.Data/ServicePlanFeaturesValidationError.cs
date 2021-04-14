using System;

namespace Microsoft.Exchange.Data
{
	public class ServicePlanFeaturesValidationError : ValidationError
	{
		public ServicePlanFeaturesValidationError(string feature, string sku) : base(DataStrings.ServicePlanFeatureCheckFailed(feature, sku))
		{
		}
	}
}
