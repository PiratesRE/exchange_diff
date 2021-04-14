using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DatabaseCopySchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ActivationPreference = new ADPropertyDefinition("ActivationPreference", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchActivationPreference", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue, DirectoryStrings.ErrorInvalidActivationPreference)
		}, null, null);

		public static readonly ADPropertyDefinition HostServer = new ADPropertyDefinition("HostServer", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchHostServerLink", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplayLag = new ADPropertyDefinition("ReplayLagTime", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), "msExchReplayLag", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.Parse("00:00:00"), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.Parse("14.00:00:00"), DirectoryStrings.ErrorReplayLagTime)
		}, null, null);

		public static readonly ADPropertyDefinition TruncationLag = new ADPropertyDefinition("TruncationLagTime", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), "msExchTruncationLag", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.Parse("00:00:00"), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.Parse("14.00:00:00"), DirectoryStrings.ErrorTruncationLagTime)
		}, null, null);

		public static readonly ADPropertyDefinition ParentObjectClass = new ADPropertyDefinition("ParentObjectClass", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMDBCopyParentClass", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseName = new ADPropertyDefinition("DatabaseName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(DatabaseCopy.DatabaseNameGetter), null, null, null);

		public static readonly ADPropertyDefinition HostServerName = new ADPropertyDefinition("HostServerName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseCopySchema.HostServer
		}, null, new GetterDelegate(DatabaseCopy.HostServerNameGetter), null, null, null);

		public static readonly ADPropertyDefinition AutoDagFlags = new ADPropertyDefinition("AutoDagFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamDatabaseCopyFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseCopyAutoActivationPolicy = new ADPropertyDefinition("DatabaseCopyAutoActivationPolicy", ExchangeObjectVersion.Exchange2010, typeof(DatabaseCopyAutoActivationPolicyType), "msExchActivationConfig", ADPropertyDefinitionFlags.None, DatabaseCopyAutoActivationPolicyType.Unrestricted, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InvalidHostServerAllowed = new ADPropertyDefinition("InvalidHostServerAllowed", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HostServerUnlinked = new ADPropertyDefinition("HostServerUnlinked", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseCopySchema.AutoDagFlags
		}, null, ADObject.FlagGetterDelegate(DatabaseCopySchema.AutoDagFlags, 2), ADObject.FlagSetterDelegate(DatabaseCopySchema.AutoDagFlags, 2), null, null);
	}
}
