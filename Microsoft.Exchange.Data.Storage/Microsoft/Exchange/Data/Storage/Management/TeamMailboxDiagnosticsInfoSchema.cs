using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxDiagnosticsInfoSchema : SimpleProviderObjectSchema
	{
		public static readonly ProviderPropertyDefinition DisplayName = new SimpleProviderPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(TeamMailboxSyncStatus), PropertyDefinitionFlags.None, TeamMailboxSyncStatus.NotAvailable, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastDocumentSyncCycleLog = new SimpleProviderPropertyDefinition("LastDocumentSyncCycleLog", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastMembershipSyncCycleLog = new SimpleProviderPropertyDefinition("LastMembershipSyncCycleLog", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastMaintenanceSyncCycleLog = new SimpleProviderPropertyDefinition("LastMaintenanceSyncCycleLog", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition DocLibSyncInfo = new SimpleProviderPropertyDefinition("DocLibSyncInfo", ExchangeObjectVersion.Exchange2010, typeof(SyncInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MembershipSyncInfo = new SimpleProviderPropertyDefinition("MembershipSyncInfo", ExchangeObjectVersion.Exchange2010, typeof(SyncInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MaintenanceSyncInfo = new SimpleProviderPropertyDefinition("MaintenanceSyncInfo", ExchangeObjectVersion.Exchange2010, typeof(SyncInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
