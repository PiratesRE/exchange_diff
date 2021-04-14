using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class MailboxAcePresentationObjectSchema : AcePresentationObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition AccessRights = new SimpleProviderPropertyDefinition("AccessRights", ExchangeObjectVersion.Exchange2003, typeof(MailboxRights[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
