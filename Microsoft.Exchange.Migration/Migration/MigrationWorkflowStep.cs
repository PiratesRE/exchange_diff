using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	public sealed class MigrationWorkflowStep : XMLSerializableBase
	{
		public MigrationWorkflowStep(MigrationStep step)
		{
			this.Step = step;
		}

		public MigrationWorkflowStep()
		{
		}

		[XmlIgnore]
		public MigrationStep Step { get; set; }

		[XmlElement("Step")]
		public int StepInt
		{
			get
			{
				return (int)this.Step;
			}
			set
			{
				this.Step = (MigrationStep)value;
			}
		}

		[XmlIgnore]
		public MigrationStage[] AllowedStages
		{
			get
			{
				MigrationStep step = this.Step;
				if (step <= MigrationStep.Provisioning)
				{
					if (step == MigrationStep.Initialization)
					{
						return InitializationStepHandler.AllowedStages;
					}
					if (step != MigrationStep.Provisioning)
					{
						goto IL_33;
					}
				}
				else if (step != MigrationStep.ProvisioningUpdate)
				{
					if (step != MigrationStep.DataMigration)
					{
						goto IL_33;
					}
					return DataMigrationStepHandler.AllowedStages;
				}
				return ProvisioningStepHandlerBase.AllowedStages;
				IL_33:
				throw new NotSupportedException("Don't know step " + this.Step);
			}
		}
	}
}
