using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging
{
	internal class ProvisioningConstraintFixStateLog : ObjectLog<ProvisioningConstraintFixStateLogEntry>
	{
		private ProvisioningConstraintFixStateLog() : base(new ProvisioningConstraintFixStateLog.ProvisioningConstraintsFixStateLogSchema(), new LoadBalanceLoggingConfig("ProvisioningConstraintFixStates"))
		{
		}

		public static void Write(DirectoryMailbox mailbox, DirectoryDatabase sourceDatabase, MoveInfo moveInfo)
		{
			ProvisioningConstraintFixStateLogEntry provisioningConstraintFixStateLogEntry = new ProvisioningConstraintFixStateLogEntry();
			provisioningConstraintFixStateLogEntry.MailboxGuid = mailbox.Guid;
			provisioningConstraintFixStateLogEntry.MailboxProvisioningHardConstraint = mailbox.MailboxProvisioningConstraints.HardConstraint;
			provisioningConstraintFixStateLogEntry.SourceDatabaseGuid = sourceDatabase.Guid;
			provisioningConstraintFixStateLogEntry.SourceDatabaseProvisioningAttributes = sourceDatabase.MailboxProvisioningAttributes;
			provisioningConstraintFixStateLogEntry.ExistingMoveStatus = moveInfo.Status;
			provisioningConstraintFixStateLogEntry.ExistingMoveRequestGuid = moveInfo.MoveRequestGuid;
			ProvisioningConstraintFixStateLog.Instance.LogObject(provisioningConstraintFixStateLogEntry);
		}

		private static readonly ProvisioningConstraintFixStateLog Instance = new ProvisioningConstraintFixStateLog();

		private class ProvisioningConstraintFixStateLogData : ConfigurableObject
		{
			public ProvisioningConstraintFixStateLogData(PropertyBag propertyBag) : base(propertyBag)
			{
			}

			internal override ObjectSchema ObjectSchema
			{
				get
				{
					return new DummyObjectSchema();
				}
			}
		}

		private class ProvisioningConstraintsFixStateLogSchema : ConfigurableObjectLogSchema<ProvisioningConstraintFixStateLog.ProvisioningConstraintFixStateLogData, DummyObjectSchema>
		{
			public override string LogType
			{
				get
				{
					return "Provisioning Constraint Fix States";
				}
			}

			public override string Software
			{
				get
				{
					return "Mailbox Load Balancing";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry> MailboxGuid = new ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry>("MailboxGuid", (ProvisioningConstraintFixStateLogEntry r) => r.MailboxGuid);

			public static readonly ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry> ExistingMoveRequestGuid = new ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry>("ExistingMoveRequestGuid", (ProvisioningConstraintFixStateLogEntry r) => r.ExistingMoveRequestGuid);

			public static readonly ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry> ExistingMoveStatus = new ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry>("ExistingMoveStatus", (ProvisioningConstraintFixStateLogEntry r) => r.ExistingMoveStatus);

			public static readonly ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry> SourceDatabaseGuid = new ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry>("SourceDatabaseGuid", (ProvisioningConstraintFixStateLogEntry r) => r.SourceDatabaseGuid);

			public static readonly ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry> SourceDatabaseProvisioningAttributes = new ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry>("SourceDatabaseProvisioningAttributes", (ProvisioningConstraintFixStateLogEntry r) => r.SourceDatabaseProvisioningAttributes);

			public static readonly ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry> MailboxProvisioningHardConstraint = new ObjectLogSimplePropertyDefinition<ProvisioningConstraintFixStateLogEntry>("MailboxProvisioningHardConstraint", (ProvisioningConstraintFixStateLogEntry r) => r.MailboxProvisioningHardConstraint);
		}
	}
}
