using System;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[Cmdlet("Start", "ComplianceSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class StartComplianceSearch : StartComplianceJob<ComplianceSearch>
	{
		[Parameter(Mandatory = false)]
		public ComplianceSearch.ComplianceSearchType Action
		{
			get
			{
				return (ComplianceSearch.ComplianceSearchType)(base.Fields["Action"] ?? ComplianceSearch.ComplianceSearchType.UnknownType);
			}
			set
			{
				base.Fields["Action"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartComplianceSearch(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ComplianceSearch complianceSearch = (ComplianceSearch)base.PrepareDataObject();
			if (this.Action != ComplianceSearch.ComplianceSearchType.UnknownType)
			{
				complianceSearch.SearchType = this.Action;
			}
			return complianceSearch;
		}

		protected override ComplianceMessage CreateStartJobMessage()
		{
			ComplianceMessage complianceMessage = base.CreateStartJobMessage();
			complianceMessage.WorkDefinitionType = WorkDefinitionType.EDiscovery;
			JobPayload jobPayload = new JobPayload();
			jobPayload.JobId = this.DataObject.JobRunId.ToString();
			jobPayload.Target = complianceMessage.MessageTarget;
			jobPayload.PayloadId = string.Empty;
			jobPayload.Children.Add(PayloadHelper.GetPayloadReference(this.DataObject.JobRunId, -1));
			jobPayload.Payload = this.DataObject.GetExchangeWorkDefinition();
			complianceMessage.Payload = ComplianceSerializer.Serialize<JobPayload>(JobPayload.Description, jobPayload);
			return complianceMessage;
		}

		private const string ParameterAction = "Action";
	}
}
