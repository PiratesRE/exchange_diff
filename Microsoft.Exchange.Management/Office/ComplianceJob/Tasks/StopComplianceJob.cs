using System;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	public abstract class StopComplianceJob<TDataObject> : ObjectActionTenantADTask<ComplianceJobIdParameter, TDataObject> where TDataObject : ComplianceJob, new()
	{
		protected override IConfigDataProvider CreateSession()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			return new ComplianceJobProvider(base.ExchangeRunspaceConfig.OrganizationId);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.ExchangeRunspaceConfig == null)
			{
				base.WriteError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			TDataObject dataObject = this.DataObject;
			switch (dataObject.JobStatus)
			{
			case ComplianceJobStatus.Starting:
			case ComplianceJobStatus.InProgress:
				break;
			default:
			{
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(new ComplianceJobTaskException(Strings.CannotStopNonRunningJob(dataObject2.Name)), ErrorCategory.InvalidOperation, this.DataObject);
				break;
			}
			}
			TDataObject dataObject3 = this.DataObject;
			dataObject3.JobEndTime = DateTime.UtcNow;
			TDataObject dataObject4 = this.DataObject;
			dataObject4.JobStatus = ComplianceJobStatus.Stopped;
			base.DataSession.Save(this.DataObject);
			TaskLogger.LogExit();
		}
	}
}
