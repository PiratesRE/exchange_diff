using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class FailedMSOSyncObjectSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition RawObjectId = new ADPropertyDefinition("RawObjectId", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchExternalDirectoryObjectId", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RawContextId = new ADPropertyDefinition("RawContextId", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchExternalDirectoryOrganizationId", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectId = new ADPropertyDefinition("ExternalDirectoryObjectId", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.RawObjectId
		}, new CustomFilterBuilderDelegate(FailedMSOSyncObject.ExternalDirectoryObjectIdFilterBuilder), new GetterDelegate(FailedMSOSyncObject.ExternalDirectoryObjectIdGetter), new SetterDelegate(FailedMSOSyncObject.ExternalDirectoryObjectIdSetter), null, null);

		public static readonly ADPropertyDefinition ContextId = new ADPropertyDefinition("ExternalDirectoryOrganizationId", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.RawContextId
		}, new CustomFilterBuilderDelegate(FailedMSOSyncObject.ExternalDirectoryOrganizationIdFilterBuilder), new GetterDelegate(FailedMSOSyncObject.ExternalDirectoryOrganizationIdGetter), new SetterDelegate(FailedMSOSyncObject.ExternalDirectoryOrganizationIdSetter), null, null);

		public static readonly ADPropertyDefinition ExternalDirectoryObjectClass = new ADPropertyDefinition("ExternalDirectoryObjectClass", ExchangeObjectVersion.Exchange2010, typeof(DirectoryObjectClass), "msExchExternalDirectoryObjectClass", ADPropertyDefinitionFlags.WriteOnce, DirectoryObjectClass.Account, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SyncObjectId = new ADPropertyDefinition("SyncObjectId", ExchangeObjectVersion.Exchange2010, typeof(SyncObjectId), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.RawObjectId,
			FailedMSOSyncObjectSchema.RawContextId,
			FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass
		}, null, (IPropertyBag bag) => new SyncObjectId((string)FailedMSOSyncObject.ExternalDirectoryOrganizationIdGetter(bag), (string)FailedMSOSyncObject.ExternalDirectoryObjectIdGetter(bag), (DirectoryObjectClass)bag[FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass]), delegate(object value, IPropertyBag bag)
		{
			SyncObjectId syncObjectId = (SyncObjectId)value;
			FailedMSOSyncObject.ExternalDirectoryOrganizationIdSetter(syncObjectId.ContextId, bag);
			FailedMSOSyncObject.ExternalDirectoryObjectIdSetter(syncObjectId.ObjectId, bag);
			bag[FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass] = syncObjectId.ObjectClass;
		}, null, null);

		public static readonly ADPropertyDefinition DivergenceTimestampRaw = new ADPropertyDefinition("DivergenceTimestampRaw", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchMSOForwardSyncDivergenceTimestamp", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DivergenceTimestamp = new ADPropertyDefinition("DivergenceTimestamp", ExchangeObjectVersion.Exchange2010, typeof(DateTime), null, ADPropertyDefinitionFlags.Calculated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.DivergenceTimestampRaw
		}, new CustomFilterBuilderDelegate(FailedMSOSyncObject.DivergenceTimestampFilterBuilder), (IPropertyBag bag) => DateTime.FromFileTimeUtc((long)bag[FailedMSOSyncObjectSchema.DivergenceTimestampRaw]), delegate(object value, IPropertyBag bag)
		{
			bag[FailedMSOSyncObjectSchema.DivergenceTimestampRaw] = ((DateTime)value).ToFileTimeUtc();
		}, null, null);

		public static readonly ADPropertyDefinition DivergenceCount = new ADPropertyDefinition("DivergenceCount", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchMSOForwardSyncDivergenceCount", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProvisioningFlags = SharedPropertyDefinitions.ProvisioningFlags;

		public static readonly ADPropertyDefinition IsTemporary = new ADPropertyDefinition("IsTemporary", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 1UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 1), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 1), null, null);

		public static readonly ADPropertyDefinition IsIncrementalOnly = new ADPropertyDefinition("IsIncrementalOnly", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 2UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 2), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 2), null, null);

		public static readonly ADPropertyDefinition IsLinkRelated = new ADPropertyDefinition("IsLinkRelated", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 4UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 4), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 4), null, null);

		public static readonly ADPropertyDefinition IsIgnoredInHaltCondition = new ADPropertyDefinition("IsIgnoredInHaltCondition", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 8UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 8), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 8), null, null);

		public static readonly ADPropertyDefinition IsTenantWideDivergence = new ADPropertyDefinition("IsTenantWideDivergence", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 16UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 16), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 16), null, null);

		public static readonly ADPropertyDefinition IsValidationDivergence = new ADPropertyDefinition("IsValidationDivergence", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 32UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 32), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 32), null, null);

		public static readonly ADPropertyDefinition IsRetriable = new ADPropertyDefinition("IsRetriable", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FailedMSOSyncObjectSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(FailedMSOSyncObjectSchema.ProvisioningFlags, 64UL)), ADObject.FlagGetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 64), ADObject.FlagSetterDelegate(FailedMSOSyncObjectSchema.ProvisioningFlags, 64), null, null);

		public static readonly ADPropertyDefinition Errors = new ADPropertyDefinition("Errors", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchRecipientValidatorCookies", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DivergenceInfoXml = new ADPropertyDefinition("DivergenceInfoXml", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchConfigurationXML", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = new ADPropertyDefinition("Comment", ExchangeObjectVersion.Exchange2003, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
