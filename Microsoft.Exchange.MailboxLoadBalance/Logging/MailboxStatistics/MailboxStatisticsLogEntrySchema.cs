using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxStatisticsLogEntrySchema : ObjectSchema
	{
		public static SimpleProviderPropertyDefinition CreatePropertyDefinition<TProperty>(string propName)
		{
			return new SimpleProviderPropertyDefinition(propName, ExchangeObjectVersion.Exchange2003, typeof(TProperty), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public static readonly SimpleProviderPropertyDefinition AttachmentTableTotalSizeInBytes = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("AttachmentTableTotalSizeInBytes");

		public static readonly SimpleProviderPropertyDefinition DatabaseName = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<string>("DatabaseName");

		public static readonly SimpleProviderPropertyDefinition DeletedItemCount = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("DeletedItemCount");

		public static readonly SimpleProviderPropertyDefinition DisconnectDate = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<DateTime?>("DisconnectDate");

		public static readonly SimpleProviderPropertyDefinition MailboxState = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<MailboxState?>("MailboxState");

		public static readonly SimpleProviderPropertyDefinition ExternalDirectoryOrganizationId = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<Guid?>("ExternalDirectoryOrganizationId");

		public static readonly SimpleProviderPropertyDefinition IsArchiveMailbox = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<bool?>("IsArchiveMailbox");

		public static readonly SimpleProviderPropertyDefinition IsMoveDestination = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<bool?>("IsMoveDestination");

		public static readonly SimpleProviderPropertyDefinition IsQuarantined = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<bool?>("IsQuarantined");

		public static readonly SimpleProviderPropertyDefinition ItemCount = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("ItemCount");

		public static readonly SimpleProviderPropertyDefinition LastLogonTime = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<DateTime?>("LastLogonTime");

		public static readonly SimpleProviderPropertyDefinition LogicalSizeInM = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("LogicalSizeInM");

		public static readonly SimpleProviderPropertyDefinition MailboxGuid = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<Guid?>("MailboxGuid");

		public static readonly SimpleProviderPropertyDefinition RecipientGuid = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<Guid?>("RecipientGuid");

		public static readonly SimpleProviderPropertyDefinition MailboxType = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<LoadBalanceMailboxType?>("MailboxType");

		public static readonly SimpleProviderPropertyDefinition MessageTableTotalSizeInBytes = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("MessageTableTotalSizeInBytes");

		public static readonly SimpleProviderPropertyDefinition OtherTablesTotalSizeInBytes = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("OtherTablesTotalSizeInBytes");

		public static readonly SimpleProviderPropertyDefinition PhysicalSizeInM = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("PhysicalSizeInM");

		public static readonly SimpleProviderPropertyDefinition TotalDeletedItemSizeInBytes = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("TotalDeletedItemSizeInBytes");

		public static readonly SimpleProviderPropertyDefinition TotalItemSizeInBytes = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<ulong?>("TotalItemSizeInBytes");

		public static readonly SimpleProviderPropertyDefinition CreationTimestamp = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<DateTime?>("CreationTimestamp");

		public static readonly SimpleProviderPropertyDefinition MailboxProvisioningConstraint = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<string>("MailboxProvisioningConstraint");

		public static readonly SimpleProviderPropertyDefinition MailboxProvisioningPreferences = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<string>("MailboxProvisioningPreferences");

		public static readonly SimpleProviderPropertyDefinition MailboxProvisioningConstraintReason = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<string>("MailboxProvisioningConstraintReason");

		public static readonly SimpleProviderPropertyDefinition ItemsPendingUpgrade = MailboxStatisticsLogEntrySchema.CreatePropertyDefinition<int?>("ItemsPendingUpgrade");

		internal class MailboxStatisticsLogSchema : ConfigurableObjectLogSchema<MailboxStatisticsLogEntry, MailboxStatisticsLogEntrySchema>
		{
			public override string LogType
			{
				get
				{
					return "Mailbox Statistics";
				}
			}

			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Load Balancing";
				}
			}
		}
	}
}
