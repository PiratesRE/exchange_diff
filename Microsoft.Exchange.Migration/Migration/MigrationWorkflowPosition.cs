using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationWorkflowPosition : IMigrationSerializable
	{
		public MigrationWorkflowPosition(MigrationStep step, MigrationStage stage)
		{
			this.Step = step;
			this.Stage = stage;
		}

		private MigrationWorkflowPosition()
		{
		}

		public MigrationStep Step { get; private set; }

		public MigrationStage Stage { get; private set; }

		public PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MigrationWorkflowPosition.MigrationWorkflowPositionProperties;
			}
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.Stage = MigrationHelper.GetEnumProperty<MigrationStage>(message, MigrationBatchMessageSchema.MigrationStage);
			this.Step = MigrationHelper.GetEnumProperty<MigrationStep>(message, MigrationBatchMessageSchema.MigrationStep);
			return true;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[MigrationBatchMessageSchema.MigrationStep] = this.Step;
			message[MigrationBatchMessageSchema.MigrationStage] = this.Stage;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationWorkflowPosition");
			xelement.Add(new object[]
			{
				new XElement("Step", this.Step),
				new XElement("Stage", this.Stage)
			});
			return xelement;
		}

		public override string ToString()
		{
			return this.Step + "," + this.Stage;
		}

		internal static MigrationWorkflowPosition CreateFromMessage(IMigrationStoreObject message)
		{
			MigrationWorkflowPosition migrationWorkflowPosition = new MigrationWorkflowPosition();
			migrationWorkflowPosition.ReadFromMessageItem(message);
			return migrationWorkflowPosition;
		}

		internal static IStepHandler CreateStepHandler(MigrationWorkflowPosition position, IMigrationDataProvider dataProvider, MigrationJob migrationJob)
		{
			MigrationStep step = position.Step;
			if (step <= MigrationStep.Provisioning)
			{
				if (step == MigrationStep.Initialization)
				{
					return new InitializationStepHandler(dataProvider);
				}
				if (step == MigrationStep.Provisioning)
				{
					return new ProvisioningStepHandler(dataProvider);
				}
			}
			else
			{
				if (step == MigrationStep.ProvisioningUpdate)
				{
					return new ProvisioningUpdateStepHandler(dataProvider);
				}
				if (step == MigrationStep.DataMigration)
				{
					return new DataMigrationStepHandler(dataProvider, migrationJob.MigrationType, migrationJob.JobName);
				}
			}
			throw new NotSupportedException(string.Format("Step {0} not yet supported", position.Step));
		}

		internal static ISnapshotId GetStepSnapshotId(MigrationWorkflowPosition position, MigrationJobItem jobItem)
		{
			MigrationStep step = position.Step;
			if (step <= MigrationStep.Provisioning)
			{
				if (step == MigrationStep.Initialization)
				{
					goto IL_2F;
				}
				if (step != MigrationStep.Provisioning)
				{
					goto IL_2F;
				}
			}
			else if (step != MigrationStep.ProvisioningUpdate)
			{
				if (step != MigrationStep.DataMigration)
				{
					goto IL_2F;
				}
				return jobItem.SubscriptionId;
			}
			return jobItem.ProvisioningId;
			IL_2F:
			return null;
		}

		internal MigrationUserStatus GetInitialStatus()
		{
			MigrationStep step = this.Step;
			if (step <= MigrationStep.Provisioning)
			{
				if (step == MigrationStep.Initialization)
				{
					return MigrationUserStatus.Validating;
				}
				if (step == MigrationStep.Provisioning)
				{
					return MigrationUserStatus.Provisioning;
				}
			}
			else
			{
				if (step == MigrationStep.ProvisioningUpdate)
				{
					return MigrationUserStatus.ProvisionUpdating;
				}
				if (step == MigrationStep.DataMigration)
				{
					return MigrationUserStatus.Syncing;
				}
			}
			throw new NotSupportedException(string.Format("Step {0} not yet supported", this.Step));
		}

		internal static readonly PropertyDefinition[] MigrationWorkflowPositionProperties = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationStep,
			MigrationBatchMessageSchema.MigrationStage
		};
	}
}
