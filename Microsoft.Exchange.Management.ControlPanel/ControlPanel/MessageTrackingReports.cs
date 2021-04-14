using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MessageTrackingReports : DataSourceService, IMessageTrackingReport, IGetListService<RecipientTrackingEventsFilter, RecipientStatusRow>, IGetObjectService<MessageTrackingReportRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MessageTrackingReport?ResultSize&Identity&Status&Recipients@R:Self")]
		public PowerShellResults<RecipientStatusRow> GetList(RecipientTrackingEventsFilter filter, SortOptions sort)
		{
			PowerShellResults<MessageTrackingReportRow> list = base.GetList<MessageTrackingReportRow, RecipientTrackingEventsFilter>("Get-MessageTrackingReport", filter, null);
			PowerShellResults<RecipientStatusRow> powerShellResults = new PowerShellResults<RecipientStatusRow>();
			powerShellResults.MergeErrors<MessageTrackingReportRow>(list);
			if (list.SucceededWithValue)
			{
				powerShellResults.Output = list.Value.RecipientStatuses;
			}
			else
			{
				powerShellResults.Output = new RecipientStatusRow[0];
			}
			if (sort != null && powerShellResults.Output.Length > 1)
			{
				Func<RecipientStatusRow[], RecipientStatusRow[]> sortFunction = sort.GetSortFunction<RecipientStatusRow>();
				powerShellResults.Output = sortFunction(powerShellResults.Output);
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MessageTrackingReport?ResultSize&Identity&Recipients@R:Self")]
		public PowerShellResults<MessageTrackingReportRow> GetObject(Identity identity)
		{
			RecipientMessageTrackingReportId recipientMessageTrackingReportId = RecipientMessageTrackingReportId.Parse(identity);
			GetMessageTrackingReportParameters getMessageTrackingReportParameters = new GetMessageTrackingReportParameters();
			getMessageTrackingReportParameters.Identity = recipientMessageTrackingReportId.MessageTrackingReportId;
			getMessageTrackingReportParameters.ResultSize = 30;
			getMessageTrackingReportParameters.ByPassDelegateChecking = true;
			getMessageTrackingReportParameters.DetailLevel = MessageTrackingDetailLevel.Verbose;
			if (!string.IsNullOrEmpty(recipientMessageTrackingReportId.Recipient))
			{
				getMessageTrackingReportParameters.Recipients = recipientMessageTrackingReportId.Recipient;
			}
			PSCommand psCommand = new PSCommand().AddCommand("Get-MessageTrackingReport").AddParameters(getMessageTrackingReportParameters);
			return base.Invoke<MessageTrackingReportRow>(psCommand);
		}

		internal const string GetCmdlet = "Get-MessageTrackingReport";

		internal const string ReadScope = "@R:Self";

		internal const int MaxResultSize = 30;

		private const string GetListRole = "Get-MessageTrackingReport?ResultSize&Identity&Status&Recipients@R:Self";

		private const string GetObjectRole = "Get-MessageTrackingReport?ResultSize&Identity&Recipients@R:Self";
	}
}
