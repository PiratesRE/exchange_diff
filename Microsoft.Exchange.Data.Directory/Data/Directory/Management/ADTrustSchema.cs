using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ADTrustSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADObjectSchema>();
		}
	}
}
