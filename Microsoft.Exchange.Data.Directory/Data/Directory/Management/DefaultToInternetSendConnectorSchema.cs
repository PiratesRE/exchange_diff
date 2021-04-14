using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class DefaultToInternetSendConnectorSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADObjectSchema>();
		}
	}
}
