using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SystemMailboxPresentationObjectSchema : MailEnabledRecipientSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADSystemMailboxSchema>();
		}
	}
}
