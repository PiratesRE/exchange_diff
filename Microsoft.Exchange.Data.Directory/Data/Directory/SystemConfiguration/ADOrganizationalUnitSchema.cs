using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADOrganizationalUnitSchema : ADConfigurationObjectSchema
	{
		internal const int MSOSyncEnabledShift = 0;

		internal const int SMTPAddressCheckWithAcceptedDomainShift = 1;

		internal const int SyncMBXAndDLToMservShift = 2;

		internal const int RelocationInProgressShift = 3;

		public static readonly ADPropertyDefinition ConfigurationUnitLink = new ADPropertyDefinition("ConfigurationUnitLink", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchConfigurationUnitBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public new static readonly ADPropertyDefinition OrganizationId = new ADPropertyDefinition("OrganizationId", ExchangeObjectVersion.Exchange2003, typeof(OrganizationId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOrganizationalUnitSchema.ConfigurationUnitLink,
			ADObjectSchema.OrganizationalUnitRoot,
			ADObjectSchema.ConfigurationUnit
		}, null, new GetterDelegate(ADOrganizationalUnit.OuOrganizationIdGetter), null, null, null);

		public static readonly ADPropertyDefinition UPNSuffixes = SharedPropertyDefinitions.UPNSuffixes;

		public static readonly ADPropertyDefinition MServSyncConfigFlags = new ADPropertyDefinition("MServSyncConfigFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchEdgeSyncConfigFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MSOSyncEnabled = ADObject.BitfieldProperty("MSOSyncEnabled", 0, ADOrganizationalUnitSchema.MServSyncConfigFlags);

		public static readonly ADPropertyDefinition SMTPAddressCheckWithAcceptedDomain = ADObject.BitfieldProperty("SMTPAddressCheckWithAcceptedDomain", 1, ADOrganizationalUnitSchema.MServSyncConfigFlags);

		public static readonly ADPropertyDefinition SyncMBXAndDLToMserv = ADObject.BitfieldProperty("SyncMBXAndDLToMserv", 2, ADOrganizationalUnitSchema.MServSyncConfigFlags);

		public static readonly ADPropertyDefinition RelocationInProgress = ADObject.BitfieldProperty("RelocationInProgress", 3, ADOrganizationalUnitSchema.MServSyncConfigFlags);
	}
}
