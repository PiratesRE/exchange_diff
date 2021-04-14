using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MessageCategorySchema : XsoMailboxConfigurationObjectSchema
	{
		public const string NameParameterName = "Name";

		public const string ColorParameterName = "Color";

		public const string GuidParameterName = "Guid";

		public const string IdentityParameterName = "Identity";

		public const string CategoryIdentityParameterName = "CategoryIdentity";

		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new List<PropertyDefinitionConstraint>(CategorySchema.Name.Constraints).ToArray());

		public static readonly SimpleProviderPropertyDefinition Color = new SimpleProviderPropertyDefinition("Color", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, new List<PropertyDefinitionConstraint>(CategorySchema.Color.Constraints).ToArray());

		public static readonly SimpleProviderPropertyDefinition Guid = new SimpleProviderPropertyDefinition("Guid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.None, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(MessageCategoryId), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageCategorySchema.Guid,
			XsoMailboxConfigurationObjectSchema.MailboxOwnerId
		}, null, new GetterDelegate(MessageCategory.IdentityGetter), null);
	}
}
