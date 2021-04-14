using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging.SoftDeletedMailboxRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMailboxRemovalLogEntrySchema : ObjectSchema
	{
		public static readonly ProviderPropertyDefinition CurrentDatabaseSize = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ByteQuantifiedSize?>("CurrentDatabaseSize");

		public static readonly ProviderPropertyDefinition DatabaseName = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<string>("DatabaseName");

		public static readonly ProviderPropertyDefinition Error = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<Exception>("Error");

		public static readonly ProviderPropertyDefinition MailboxGuid = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<Guid?>("MailboxGuid");

		public static readonly ProviderPropertyDefinition OriginalDatabaseSize = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ByteQuantifiedSize?>("OriginalDatabaseSize");

		public static readonly ProviderPropertyDefinition RemovalAllowed = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<bool?>("RemovalAllowed");

		public static readonly ProviderPropertyDefinition RemovalDisallowedReason = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<string>("RemovalDisallowedReason");

		public static readonly ProviderPropertyDefinition Removed = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<bool?>("Removed");

		public static readonly ProviderPropertyDefinition TargetDatabaseSize = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ByteQuantifiedSize?>("TargetDatabaseSize");

		public static readonly ProviderPropertyDefinition MailboxSize = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ByteQuantifiedSize?>("MailboxSize");

		internal class SoftDeletedMailboxRemovalLogEntryLogSchema : ConfigurableObjectLogSchema<SoftDeletedMailboxRemovalLogEntry, SoftDeletedMailboxRemovalLogEntrySchema>
		{
			public override string LogType
			{
				get
				{
					return "Soft Deleted Mailbox Removal";
				}
			}
		}
	}
}
