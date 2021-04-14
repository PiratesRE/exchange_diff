using System;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class OWAMiniRecipientSchema : StorageMiniRecipientSchema
	{
		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition WebPage = ADRecipientSchema.WebPage;

		public static readonly ADPropertyDefinition ActiveSyncEnabled = ADUserSchema.ActiveSyncEnabled;

		public static readonly ADPropertyDefinition ExternalOofOptions = ADMailboxRecipientSchema.ExternalOofOptions;

		public static readonly ADPropertyDefinition RecipientDisplayType = ADRecipientSchema.RecipientDisplayType;

		public static readonly ADPropertyDefinition MobilePhoneNumber = ADUserSchema.MobilePhone;

		internal static readonly ADPropertyDefinition[] AdditionalProperties = new ADPropertyDefinition[]
		{
			OWAMiniRecipientSchema.PhoneticDisplayName,
			OWAMiniRecipientSchema.WebPage,
			OWAMiniRecipientSchema.ActiveSyncEnabled,
			MiniRecipientSchema.EmailAddresses,
			OWAMiniRecipientSchema.ExternalOofOptions,
			OWAMiniRecipientSchema.RecipientDisplayType,
			StorageMiniRecipientSchema.Alias,
			OWAMiniRecipientSchema.MobilePhoneNumber,
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.ImmutableId,
			ADRecipientSchema.RawOnPremisesObjectId
		};

		internal static readonly PropertyDefinition[] AdditionalPropertiesWithClientAccessRules = ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>().AllProperties.Union(OWAMiniRecipientSchema.AdditionalProperties).ToArray<PropertyDefinition>();
	}
}
