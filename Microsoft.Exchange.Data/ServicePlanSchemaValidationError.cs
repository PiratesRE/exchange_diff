using System;

namespace Microsoft.Exchange.Data
{
	public class ServicePlanSchemaValidationError : ValidationError
	{
		public ServicePlanSchemaValidationError(string schemaError) : base(DataStrings.ServicePlanSchemaCheckFailed(schemaError))
		{
		}
	}
}
