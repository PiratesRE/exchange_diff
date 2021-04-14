using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class PublicDatabasePresentationObjectSchema : MailEnabledOrgPersonSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADPublicDatabaseSchema>();
		}
	}
}
