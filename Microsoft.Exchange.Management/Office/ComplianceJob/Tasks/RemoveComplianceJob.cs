using System;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	public abstract class RemoveComplianceJob<TDataObject> : RemoveTenantADTaskBase<ComplianceJobIdParameter, TDataObject> where TDataObject : ComplianceJob, new()
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
			if (base.ExchangeRunspaceConfig == null)
			{
				base.WriteError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
				return;
			}
			if (base.DataObject != null)
			{
				TDataObject dataObject = base.DataObject;
				if (dataObject.IsRunning())
				{
					TDataObject dataObject2 = base.DataObject;
					base.WriteError(new ComplianceJobTaskException(Strings.ComplianceSearchIsInProgress(dataObject2.Name)), ErrorCategory.InvalidOperation, null);
					return;
				}
				base.InternalProcessRecord();
			}
		}
	}
}
