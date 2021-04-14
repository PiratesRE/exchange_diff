using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class StorageGroupSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition LogFolderPath = new ADPropertyDefinition("LogFolderPath", ExchangeObjectVersion.Exchange2003, typeof(NonRootLocalLongFullPath), "msExchESEParamLogFilePath", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint,
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition LogFileSize = new ADPropertyDefinition("LogFileSize", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamLogFileSize", ADPropertyDefinitionFlags.PersistDefaultValue, 1024, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SystemFolderPath = new ADPropertyDefinition("SystemFolderPath", ExchangeObjectVersion.Exchange2003, typeof(NonRootLocalLongFullPath), "msExchESEParamSystemPath", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint,
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition CircularLoggingEnabledValue = new ADPropertyDefinition("CircularLoggingEnabledValue", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamCircularLog", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ZeroDatabasePagesValue = new ADPropertyDefinition("ZeroDatabasePagesValue", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamZeroDatabaseDuringBackup", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogFilePrefix = new ADPropertyDefinition("LogFilePrefix", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchESEParamBaseName", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogCheckpointDepth = new ADPropertyDefinition("LogCheckpointDepth", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamCheckpointDepthMax", ADPropertyDefinitionFlags.PersistDefaultValue, 20971520, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CommitDefault = new ADPropertyDefinition("CommitDefault", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamCommitDefault", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseExtensionSize = new ADPropertyDefinition("DatabaseExtensionSize", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamDbExtensionSize", ADPropertyDefinitionFlags.PersistDefaultValue, 256, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IndexCheckingEnabled = new ADPropertyDefinition("IndexCheckingEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchESEParamEnableIndexChecking", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OnlineDefragEnabled = new ADPropertyDefinition("OnlineDefragEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchESEParamEnableOnlineDefrag", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EventLogSourceID = new ADPropertyDefinition("EventLogSourceID", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchESEParamEventSource", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecoveryEnabled = new ADPropertyDefinition("RecoveryEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchRecovery", ADPropertyDefinitionFlags.ReadOnly, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PageFragment = new ADPropertyDefinition("PageFragment", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamPageFragment", ADPropertyDefinitionFlags.PersistDefaultValue, 8, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PageTempDBMinimum = new ADPropertyDefinition("PageTempDBMinimum", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamPageTempDBMin", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Recovery = new ADPropertyDefinition("Recovery", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchRestore", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CanRunDefaultUpdate = new ADPropertyDefinition("CanRunDefaultUpdate", ExchangeObjectVersion.Exchange2007, typeof(CanRunDefaultUpdateState?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CanRunRestore = new ADPropertyDefinition("CanRunRestore", ExchangeObjectVersion.Exchange2007, typeof(CanRunRestoreState?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Server = new ADPropertyDefinition("Server", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(StorageGroup.ServerGetter), null, null, null);

		public static readonly ADPropertyDefinition ServerName = new ADPropertyDefinition("ServerName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(StorageGroup.ServerNameGetter), null, null, null);

		public static readonly ADPropertyDefinition CircularLoggingEnabled = new ADPropertyDefinition("CircularLoggingEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			StorageGroupSchema.CircularLoggingEnabledValue
		}, null, (IPropertyBag propertyBag) => 0 != (int)propertyBag[StorageGroupSchema.CircularLoggingEnabledValue], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[StorageGroupSchema.CircularLoggingEnabledValue] = (((bool)value) ? 1 : 0);
		}, null, null);

		public static readonly ADPropertyDefinition ZeroDatabasePages = new ADPropertyDefinition("ZeroDatabasePages", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			StorageGroupSchema.ZeroDatabasePagesValue
		}, null, (IPropertyBag propertyBag) => 0 != (int)propertyBag[StorageGroupSchema.ZeroDatabasePagesValue], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[StorageGroupSchema.ZeroDatabasePagesValue] = (((bool)value) ? 1 : 0);
		}, null, null);

		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2003, typeof(string), "name", ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ADObjectNameStringLengthConstraint(1, 64),
			new ContainingNonWhitespaceConstraint(),
			new ADObjectNameCharacterConstraint(new char[]
			{
				'\\',
				'/',
				'=',
				';',
				'\0',
				'\n'
			})
		}, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(StorageGroup.StorageGroupNameGetter), new SetterDelegate(StorageGroup.StorageGroupNameSetter), null, null);
	}
}
