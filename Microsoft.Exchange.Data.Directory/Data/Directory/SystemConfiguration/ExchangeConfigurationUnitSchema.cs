using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ExchangeConfigurationUnitSchema : OrganizationSchema
	{
		private const int RelocationLastErrorShift = 0;

		private const int RelocationLastErrorLength = 8;

		private const int HasPermanentErrorShift = 7;

		private const int SuspendedShift = 8;

		private const int SuspendedLength = 1;

		private const int LargeTenantModeShift = 9;

		private const int LargeTenantModeLength = 1;

		private const int RelocationStateRequestedShift = 24;

		private const int RelocationStateRequestedLength = 7;

		private const int AutoCompletionEnabledShift = 31;

		private const int AutoCompletionEnabledLength = 1;

		public static readonly ADPropertyDefinition OtherWellKnownObjects = SharedPropertyDefinitions.OtherWellKnownObjects;

		public static readonly ADPropertyDefinition OrganizationStatus = new ADPropertyDefinition("OrganizationStatus", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSetupStatus", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetOrganizationStatus = new ADPropertyDefinition("TargetOrganizationStatus", ExchangeObjectVersion.Exchange2003, typeof(OrganizationStatus), null, ADPropertyDefinitionFlags.TaskPopulated, Microsoft.Exchange.Data.Directory.SystemConfiguration.OrganizationStatus.PendingArrival, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDirectoryOrganizationIdRaw = new ADPropertyDefinition("ExternalDirectoryOrganizationIdRaw", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchExternalDirectoryOrganizationId", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDirectoryOrganizationId = new ADPropertyDefinition("ExternalDirectoryOrganizationId", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationIdRaw,
			OrganizationSchema.OrganizationFlags2
		}, new CustomFilterBuilderDelegate(ExchangeConfigurationUnit.ExternalDirectoryOrganizationIdFilterBuilder), new GetterDelegate(ExchangeConfigurationUnit.ExternalDirectoryOrganizationIdGetter), new SetterDelegate(ExchangeConfigurationUnit.ExternalDirectoryOrganizationIdSetter), null, null);

		public static readonly ADPropertyDefinition WhenOrganizationStatusSet = new ADPropertyDefinition("WhenOrganizationStatusSet", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), "msExchSetupTime", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		internal static readonly ADPropertyDefinition OrganizationalUnitLink = new ADPropertyDefinition("OrganizationalUnitLink", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchConfigurationUnitLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public new static readonly ADPropertyDefinition OrganizationId = new ADPropertyDefinition("OrganizationId", ExchangeObjectVersion.Exchange2007, typeof(OrganizationId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.OrganizationalUnitRoot,
			ADObjectSchema.ConfigurationUnit,
			ExchangeConfigurationUnitSchema.OrganizationalUnitLink
		}, null, new GetterDelegate(ExchangeConfigurationUnit.CuOrganizationIdGetter), null, null, null);

		public static readonly ADPropertyDefinition ServicePlan = new ADPropertyDefinition("ServicePlan", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchServicePlan", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 1023),
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition TargetServicePlan = new ADPropertyDefinition("TargetServicePlan", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchIntendedServicePlan", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 1023),
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ResellerId = new ADPropertyDefinition("ResellerId", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchReseller", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 1023),
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ProgramId = new ADPropertyDefinition("ProgramId", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.ResellerId
		}, new CustomFilterBuilderDelegate(ExchangeConfigurationUnit.ProgramIdFilterBuilder), new GetterDelegate(ExchangeConfigurationUnit.ProgramIdGetter), new SetterDelegate(ExchangeConfigurationUnit.ProgramIdSetter), null, null);

		public static readonly ADPropertyDefinition OfferId = new ADPropertyDefinition("OfferId", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.ResellerId
		}, null, new GetterDelegate(ExchangeConfigurationUnit.OfferIdGetter), new SetterDelegate(ExchangeConfigurationUnit.OfferIdSetter), null, null);

		internal static readonly ADPropertyDefinition ManagementSiteLink = new ADPropertyDefinition("ManagementSiteLink", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, "msExchManagementSiteLink", null, "msExchManagementSiteLinkSL", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		internal static readonly ADPropertyDefinition UsnCreated = new ADPropertyDefinition("UsnCreated", ExchangeObjectVersion.Exchange2007, typeof(long), "uSNCreated", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MsoForwardSyncNonRecipientCookie = new ADPropertyDefinition("MsoForwardSyncNonRecipientCookie", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchMsoForwardSyncNonRecipientCookie", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MsoForwardSyncRecipientCookie = new ADPropertyDefinition("MsoForwardSyncRecipientCookie", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchMsoForwardSyncRecipientCookie", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition DirSyncId = GALSyncOrganizationSchema.GALSyncClientData;

		[Obsolete("DefaultPublicFolderDatabase is not used by product code any more.")]
		public static readonly ADPropertyDefinition DefaultPublicFolderDatabase = new ADPropertyDefinition("DefaultPublicFolderDatabase", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchDefaultPublicMDB", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DirSyncServiceInstanceRaw = new ADPropertyDefinition("DirSyncServiceInstanceRaw", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDirSyncServiceInstance", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DirSyncServiceInstance = new ADPropertyDefinition("DirSyncServiceInstance", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.DirSyncServiceInstanceRaw
		}, new CustomFilterBuilderDelegate(ExchangeConfigurationUnit.DirSyncServiceInstanceFilterBuilder), (IPropertyBag bag) => bag[ExchangeConfigurationUnitSchema.DirSyncServiceInstanceRaw], delegate(object value, IPropertyBag bag)
		{
			bag[ExchangeConfigurationUnitSchema.DirSyncServiceInstanceRaw] = ((value != null) ? value.ToString().ToLower() : null);
		}, null, null);

		public static readonly ADPropertyDefinition ExchangeUpgradeBucket = new ADPropertyDefinition("ExchangeUpgradeBucket", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchOrganizationUpgradePolicyLink", null, "msExchOrganizationUpgradePolicyLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition TenantRelocationCompletionTargetVector = new ADPropertyDefinition("TenantRelocationCompletionTargetVector", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchRelocateTenantCompletionTargetVector", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SafeLockdownSchedule = new ADPropertyDefinition("SafeLockdownSchedule", ExchangeObjectVersion.Exchange2003, typeof(Schedule), "msExchRelocateTenantSafeLockdownSchedule", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RelocationSourceForestRaw = new ADPropertyDefinition("RelocationSourceForestRaw", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchRelocateTenantSourceForest", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetForest = new ADPropertyDefinition("TargetForest", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchRelocateTenantTargetForest", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RelocationStatusDetailsRaw = new ADPropertyDefinition("RelocationStatusDetailsRaw", ExchangeObjectVersion.Exchange2003, typeof(RelocationStatusDetails), "msExchRelocateTenantStatus", ADPropertyDefinitionFlags.None, RelocationStatusDetails.NotStarted, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RelocationStatus = new ADPropertyDefinition("RelocationStatus", ExchangeObjectVersion.Exchange2003, typeof(TenantRelocationStatus), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, TenantRelocationStatus.NotStarted, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(TenantRelocationStatus))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.RelocationStatusDetailsRaw
		}, null, new GetterDelegate(TenantRelocationRequest.GetRelocationStatus), null, null, null);

		public static readonly ADPropertyDefinition RelocationStatusDetailsSource = new ADPropertyDefinition("RelocationStatusDetailsSource", ExchangeObjectVersion.Exchange2003, typeof(RelocationStatusDetailsSource), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.SystemConfiguration.RelocationStatusDetailsSource.NotStarted, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeConfigurationUnitSchema.RelocationStatusDetailsRaw
		}, null, new GetterDelegate(TenantRelocationRequest.GetRelocationStatusDetailsSource), null, null, null);

		public static readonly ADPropertyDefinition RelocationStatusDetailsDestination = new ADPropertyDefinition("RelocationStatusDetailsDestination", ExchangeObjectVersion.Exchange2003, typeof(RelocationStatusDetailsDestination), null, ADPropertyDefinitionFlags.TaskPopulated, Microsoft.Exchange.Data.Directory.SystemConfiguration.RelocationStatusDetailsDestination.NotStarted, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RelocationStatusDetailsDestination))
		}, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RelocationStatusDetailsDestination))
		}, null, null);

		public static readonly ADPropertyDefinition SourceForest = new ADPropertyDefinition("SourceForest", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LastSuccessfulRelocationSyncStart = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, DateTime?>("LastSuccessfulRelocationSyncStart", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.LastSuccessfulRelocationSyncStart, delegate(OrganizationConfigXML configXml, DateTime? value)
		{
			configXml.LastSuccessfulRelocationSyncStart = value;
		}, null, null);

		public static readonly ADPropertyDefinition TenantRelocationFlags = new ADPropertyDefinition("TenantRelocationFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchRelocateTenantFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RelocationStateRequested = ADObject.BitfieldProperty("RelocationStateRequested", 24, 7, ExchangeConfigurationUnitSchema.TenantRelocationFlags);

		public static readonly ADPropertyDefinition Suspended = ADObject.BitfieldProperty("Suspended", 8, ExchangeConfigurationUnitSchema.TenantRelocationFlags);

		public static readonly ADPropertyDefinition RelocationLastError = ADObject.BitfieldProperty("RelocationLastError", 0, 8, ExchangeConfigurationUnitSchema.TenantRelocationFlags);

		public static readonly ADPropertyDefinition HasPermanentError = ADObject.BitfieldProperty("HasPermanentError", 7, ExchangeConfigurationUnitSchema.TenantRelocationFlags);

		public static readonly ADPropertyDefinition AutoCompletionEnabled = ADObject.BitfieldProperty("AutoCompletionEnabled", 31, ExchangeConfigurationUnitSchema.TenantRelocationFlags);

		public static readonly ADPropertyDefinition LargeTenantModeEnabled = ADObject.BitfieldProperty("LargeTenantModeEnabled", 9, ExchangeConfigurationUnitSchema.TenantRelocationFlags);

		public static readonly ADPropertyDefinition OrganizationName = new ADPropertyDefinition("OrganizationName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.LegacyExchangeDN
		}, new CustomFilterBuilderDelegate(ExchangeConfigurationUnit.OrganizationNameFilterBuilder), new GetterDelegate(ExchangeConfigurationUnit.OrganizationNameGetter), null, null, null);

		public static readonly ADPropertyDefinition IOwnMigrationTenant = new ADPropertyDefinition("IOwnMigrationTenant", ExchangeObjectVersion.Exchange2003, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IOwnMigrationStatusReport = new ADPropertyDefinition("IOwnMigrationStatusReport", ExchangeObjectVersion.Exchange2003, typeof(string), "Description", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IOwnMigrationStatus = new ADPropertyDefinition("IOwnMigrationStatus", ExchangeObjectVersion.Exchange2003, typeof(IOwnMigrationStatusFlagsEnum), "msExchShadowCompany", ADPropertyDefinitionFlags.None, IOwnMigrationStatusFlagsEnum.NotStarted, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(IOwnMigrationStatusFlagsEnum))
		}, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(IOwnMigrationStatusFlagsEnum))
		}, null, null);
	}
}
