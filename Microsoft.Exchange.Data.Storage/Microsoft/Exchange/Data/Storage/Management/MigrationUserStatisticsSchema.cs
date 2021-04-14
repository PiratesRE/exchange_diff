using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationUserStatisticsSchema : MigrationUserSchema
	{
		public new static readonly ProviderPropertyDefinition Identity = MigrationUserSchema.Identity;

		public static readonly ProviderPropertyDefinition TotalItemsInSourceMailboxCount = new SimpleProviderPropertyDefinition("TotalItemsInSourceMailboxCount", ExchangeObjectVersion.Exchange2010, typeof(long?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TotalQueuedDuration = new SimpleProviderPropertyDefinition("TotalQueuedDuration", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TotalInProgressDuration = new SimpleProviderPropertyDefinition("TotalInProgressDuration", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TotalSyncedDuration = new SimpleProviderPropertyDefinition("TotalSyncedDuration", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TotalStalledDuration = new SimpleProviderPropertyDefinition("TotalStalledDuration", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Error = new SimpleProviderPropertyDefinition("Error", ExchangeObjectVersion.Exchange2010, typeof(object), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition DiagnosticInfo = new SimpleProviderPropertyDefinition("DiagnosticInfo", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MigrationType = new SimpleProviderPropertyDefinition("MigrationType", ExchangeObjectVersion.Exchange2010, typeof(MigrationType), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Storage.Management.MigrationType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition EstimatedTotalTransferSize = new SimpleProviderPropertyDefinition("EstimatedTotalTransferSize", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition EstimatedTotalTransferCount = new SimpleProviderPropertyDefinition("EstimatedTotalTransferCount", ExchangeObjectVersion.Exchange2010, typeof(ulong?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition BytesTransferred = new SimpleProviderPropertyDefinition("BytesTransferred", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition AverageBytesTransferredPerHour = new SimpleProviderPropertyDefinition("AverageBytesTransferredPerHour", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CurrentBytesTransferredPerMinute = new SimpleProviderPropertyDefinition("CurrentBytesTransferredPerMinute", ExchangeObjectVersion.Exchange2010, typeof(ByteQuantifiedSize?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition PercentageComplete = new SimpleProviderPropertyDefinition("PercentageComplete", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SkippedItems = new SimpleProviderPropertyDefinition("SkippedItems", ExchangeObjectVersion.Exchange2010, typeof(MigrationUserSkippedItem), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
