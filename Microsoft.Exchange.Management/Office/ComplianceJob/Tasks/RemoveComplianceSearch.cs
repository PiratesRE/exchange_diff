using System;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[Cmdlet("Remove", "ComplianceSearch", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveComplianceSearch : RemoveComplianceJob<ComplianceSearch>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveComplianceSearchConfirmation(base.DataObject.Name);
			}
		}
	}
}
