using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class EdgeSubscriptionSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADObjectSchema>();
		}
	}
}
