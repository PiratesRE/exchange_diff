using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderMailboxMonitoringInfoSchema : SimpleProviderObjectSchema
	{
		internal const string NumberOfBatchesExecuted = "NumberOfBatchesExecuted";

		internal const string NumberOfFoldersToBeSynced = "NumberOfFoldersToBeSynced";

		internal const string NumberOfFoldersSynced = "NumberOfFoldersSynced";

		internal const string BatchSize = "BatchSize";

		internal const string LastAttemptedSyncTimeFieldName = "LastAttemptedSyncTime";

		internal const string LastSuccessfulSyncTimeFieldName = "LastSuccessfulSyncTime";

		internal const string LastFailedSyncTimeFieldName = "LastFailedSyncTime";

		internal const string LastSyncFailureFieldName = "LastSyncFailure";

		internal const string NumberofAttemptsAfterLastSuccessFieldName = "NumberofAttemptsAfterLastSuccess";

		internal const string FirstFailedSyncTimeAfterLastSuccessFieldName = "FirstFailedSyncTimeAfterLastSuccess";

		internal const string DiagnosticsInfoFieldName = "DiagnosticsInfo";

		public static readonly ProviderPropertyDefinition DisplayName = new SimpleProviderPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastAttemptedSyncTime = new SimpleProviderPropertyDefinition("LastAttemptedSyncTime", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastSuccessfulSyncTime = new SimpleProviderPropertyDefinition("LastSuccessfulSyncTime", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastFailedSyncTime = new SimpleProviderPropertyDefinition("LastFailedSyncTime", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastSyncFailure = new SimpleProviderPropertyDefinition("LastSyncFailure", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition NumberofAttemptsAfterLastSuccess = new SimpleProviderPropertyDefinition("NumberofAttemptsAfterLastSuccess", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FirstFailedSyncTimeAfterLastSuccess = new SimpleProviderPropertyDefinition("FirstFailedSyncTimeAfterLastSuccess", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition DiagnosticsInfo = new SimpleProviderPropertyDefinition("DiagnosticsInfo", ExchangeObjectVersion.Exchange2010, typeof(PublicFolderMailboxDiagnosticsInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastSyncCycleLog = new SimpleProviderPropertyDefinition("LastSyncCycleLog", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
