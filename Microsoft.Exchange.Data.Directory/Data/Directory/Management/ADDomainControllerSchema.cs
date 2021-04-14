using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ADDomainControllerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADObjectSchema>();
		}
	}
}
