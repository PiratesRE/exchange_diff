using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	internal class DeletedObjectSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition IsDeleted = new ADPropertyDefinition("IsDeleted", ExchangeObjectVersion.Exchange2003, typeof(bool), "isDeleted", ADPropertyDefinitionFlags.FilterOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, MServRecipientSchema.IsDeleted, null);

		public static readonly ADPropertyDefinition EndOfList = SharedPropertyDefinitions.EndOfList;

		public static readonly ADPropertyDefinition Cookie = SharedPropertyDefinitions.Cookie;

		public static ADPropertyDefinition LastKnownParent = new ADPropertyDefinition("LastKnownParent", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "lastKnownParent", ADPropertyDefinitionFlags.ReadOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
