using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MessageTrackingSearch : DataSourceService, IMessageTrackingSearch, IGetListService<MessageTrackingSearchFilter, MessageTrackingSearchResultRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-MessageTrackingReport?ResultSize&Identity&MessageId&Subject&Sender&Recipients@R:Self")]
		public PowerShellResults<MessageTrackingSearchResultRow> GetList(MessageTrackingSearchFilter filter, SortOptions sort)
		{
			return base.GetList<MessageTrackingSearchResultRow, MessageTrackingSearchFilter>("Search-MessageTrackingReport", filter, sort);
		}

		internal const string GetCmdlet = "Search-MessageTrackingReport";

		internal const string ReadScope = "@R:Self";

		private const string GetListRole = "Search-MessageTrackingReport?ResultSize&Identity&MessageId&Subject&Sender&Recipients@R:Self";
	}
}
