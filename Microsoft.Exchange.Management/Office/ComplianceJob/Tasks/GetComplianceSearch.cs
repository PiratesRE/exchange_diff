using System;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[Cmdlet("Get", "ComplianceSearch", DefaultParameterSetName = "Identity")]
	public sealed class GetComplianceSearch : GetComplianceJob<ComplianceSearch>
	{
	}
}
