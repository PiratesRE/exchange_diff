using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MicrosoftExchangeRecipientPresentationObjectSchema : MailEnabledRecipientSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADMicrosoftExchangeRecipientSchema>();
		}
	}
}
