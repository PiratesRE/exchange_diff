using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	public sealed class MigrationWorkflow : XMLSerializableBase
	{
		public MigrationWorkflow(MigrationWorkflowStep[] workflowSteps)
		{
			MigrationUtil.ThrowOnCollectionEmptyArgument(workflowSteps, "workflowSteps");
			this.Steps = workflowSteps;
		}

		public MigrationWorkflow()
		{
			this.Steps = null;
		}

		[XmlArray("Workflow")]
		[XmlArrayItem("Step")]
		public MigrationWorkflowStep[] Steps { get; set; }

		internal static MigrationWorkflow Deserialize(string content)
		{
			return XMLSerializableBase.Deserialize<MigrationWorkflow>(content, true);
		}

		internal MigrationWorkflowPosition GetInitialPosition()
		{
			if (this.Steps == null || this.Steps.Length < 1)
			{
				return null;
			}
			return new MigrationWorkflowPosition(this.Steps[0].Step, this.Steps[0].AllowedStages[0]);
		}

		internal MigrationWorkflowPosition GetNextPosition(MigrationWorkflowPosition position, MigrationStep[] supportedSteps)
		{
			if (this.Steps == null || this.Steps.Length < 1)
			{
				return null;
			}
			bool flag = false;
			foreach (MigrationWorkflowStep migrationWorkflowStep in this.Steps)
			{
				if (supportedSteps.Contains(migrationWorkflowStep.Step))
				{
					if (flag || migrationWorkflowStep.Step == position.Step)
					{
						foreach (MigrationStage migrationStage in migrationWorkflowStep.AllowedStages)
						{
							if (flag)
							{
								return new MigrationWorkflowPosition(migrationWorkflowStep.Step, migrationStage);
							}
							if (migrationStage == position.Stage)
							{
								flag = true;
							}
						}
						flag = true;
					}
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "skipping step {0} which isn't supported for this job-item", new object[]
					{
						migrationWorkflowStep.Step
					});
				}
			}
			return null;
		}

		internal MigrationWorkflowPosition GetRestartPosition(MigrationWorkflowPosition position)
		{
			MigrationUtil.ThrowOnNullArgument(position, "position");
			if (this.Steps == null || this.Steps.Length < 1)
			{
				return position;
			}
			foreach (MigrationWorkflowStep migrationWorkflowStep in this.Steps)
			{
				if (migrationWorkflowStep.Step == position.Step)
				{
					return new MigrationWorkflowPosition(position.Step, migrationWorkflowStep.AllowedStages[0]);
				}
			}
			return position;
		}

		internal bool ShouldDelay(MigrationWorkflowPosition position, MigrationJob job)
		{
			return position.Step == MigrationStep.ProvisioningUpdate && job.GetItemCount(new MigrationUserStatus[]
			{
				MigrationUserStatus.Validating,
				MigrationUserStatus.Provisioning
			}) > 0;
		}

		[Conditional("DEBUG")]
		internal void ValidateWorkflowSteps(MigrationWorkflowStep[] workflowSteps)
		{
			HashSet<MigrationStep> hashSet = new HashSet<MigrationStep>();
			foreach (MigrationWorkflowStep migrationWorkflowStep in workflowSteps)
			{
				MigrationUtil.AssertOrThrow(!hashSet.Contains(migrationWorkflowStep.Step), "Workflow Step {0} is repeated!", new object[]
				{
					migrationWorkflowStep.Step
				});
				hashSet.Add(migrationWorkflowStep.Step);
			}
		}

		internal List<MigrationStep> GetRemainingSteps(MigrationWorkflowPosition position)
		{
			List<MigrationStep> list = new List<MigrationStep>();
			bool flag = false;
			foreach (MigrationWorkflowStep migrationWorkflowStep in this.Steps)
			{
				if (migrationWorkflowStep.Step == position.Step)
				{
					flag = true;
				}
				if (flag)
				{
					list.Add(migrationWorkflowStep.Step);
				}
			}
			return list;
		}

		internal static readonly MigrationWorkflowStep[] DefaultProvisionAndMigrateWorkflowSteps = new MigrationWorkflowStep[]
		{
			new MigrationWorkflowStep(MigrationStep.Initialization),
			new MigrationWorkflowStep(MigrationStep.Provisioning),
			new MigrationWorkflowStep(MigrationStep.ProvisioningUpdate),
			new MigrationWorkflowStep(MigrationStep.DataMigration)
		};

		internal static readonly MigrationWorkflowStep[] DefaultMigrationWorkflowSteps = new MigrationWorkflowStep[]
		{
			new MigrationWorkflowStep(MigrationStep.Initialization),
			new MigrationWorkflowStep(MigrationStep.DataMigration)
		};

		internal static readonly MigrationWorkflowStep[] DefaultProvisioningWorkflowSteps = new MigrationWorkflowStep[]
		{
			new MigrationWorkflowStep(MigrationStep.Initialization),
			new MigrationWorkflowStep(MigrationStep.Provisioning)
		};
	}
}
