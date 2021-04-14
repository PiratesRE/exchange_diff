using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MailboxSearches : DataSourceService, IMailboxSearches, IDataSourceService<MailboxSearchFilter, MailboxSearchRow, MailboxSearch, SetMailboxSearchParameters, NewMailboxSearchParameters>, IDataSourceService<MailboxSearchFilter, MailboxSearchRow, MailboxSearch, SetMailboxSearchParameters, NewMailboxSearchParameters, BaseWebServiceParameters>, IEditListService<MailboxSearchFilter, MailboxSearchRow, MailboxSearch, NewMailboxSearchParameters, BaseWebServiceParameters>, IGetListService<MailboxSearchFilter, MailboxSearchRow>, INewObjectService<MailboxSearchRow, NewMailboxSearchParameters>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<MailboxSearch, SetMailboxSearchParameters, MailboxSearchRow>, IGetObjectService<MailboxSearch>, IGetObjectForListService<MailboxSearchRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxSearch?Identity")]
		public PowerShellResults<MailboxSearchRow> GetList(MailboxSearchFilter filter, SortOptions sort)
		{
			return base.GetList<MailboxSearchRow, MailboxSearchFilter>("Get-MailboxSearch", filter, sort, "LastModifiedUTCDateTime");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxSearch?Identity")]
		public PowerShellResults<MailboxSearch> GetObject(Identity identity)
		{
			return base.GetObject<MailboxSearch>("Get-MailboxSearch", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxSearch?Identity")]
		public PowerShellResults<MailboxSearchRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<MailboxSearchRow>("Get-MailboxSearch", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-MailboxSearch?StartDate&EndDate&SourceMailboxes&Language")]
		public PowerShellResults<MailboxSearchRow> NewObject(NewMailboxSearchParameters properties)
		{
			return base.NewObject<MailboxSearchRow, NewMailboxSearchParameters>("New-MailboxSearch", properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-MailboxSearch?Identity")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-MailboxSearch", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxSearch?Identity+Set-MailboxSearch?Identity")]
		public PowerShellResults<MailboxSearchRow> SetObject(Identity identity, SetMailboxSearchParameters properties)
		{
			return base.SetObject<MailboxSearch, SetMailboxSearchParameters, MailboxSearchRow>("Set-MailboxSearch", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Start-MailboxSearch?Identity")]
		public PowerShellResults<MailboxSearchRow> StartSearch(Identity[] identities, StartMailboxSearchParameters parameters)
		{
			List<Identity> list = new List<Identity>();
			if (parameters != null && parameters.Resume)
			{
				list.AddRange(identities);
			}
			else
			{
				foreach (Identity identity in identities)
				{
					PowerShellResults<MailboxSearch> @object = base.GetObject<MailboxSearch>("Get-MailboxSearch", identity);
					if (@object.Succeeded && @object.HasValue)
					{
						if (!@object.Output[0].IsEstimateOnly)
						{
							PSCommand pscommand = new PSCommand().AddCommand("Set-MailboxSearch");
							pscommand.AddParameter("Identity", identity);
							pscommand.AddParameter("EstimateOnly", true);
							pscommand.AddParameter("ExcludeDuplicateMessages", false);
							pscommand.AddParameter("LogLevel", LoggingLevel.Suppress);
							pscommand.AddParameter("Force", true);
							PowerShellResults powerShellResults = base.Invoke(pscommand);
							if (!powerShellResults.Succeeded)
							{
								break;
							}
						}
						list.Add(identity);
					}
				}
			}
			return base.InvokeAndGetObject<MailboxSearchRow>(new PSCommand().AddCommand("Start-MailboxSearch"), list.ToArray(), parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Stop-MailboxSearch?Identity")]
		public PowerShellResults<MailboxSearchRow> StopSearch(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<MailboxSearchRow>(new PSCommand().AddCommand("Stop-MailboxSearch"), identities, parameters);
		}

		internal const string GetCmdlet = "Get-MailboxSearch";

		internal const string NewCmdlet = "New-MailboxSearch";

		internal const string RemoveCmdlet = "Remove-MailboxSearch";

		internal const string SetCmdlet = "Set-MailboxSearch";

		internal const string StartCmdlet = "Start-MailboxSearch";

		internal const string StopCmdlet = "Stop-MailboxSearch";

		private const string GetListRole = "Get-MailboxSearch";

		private const string GetObjectRole = "Get-MailboxSearch?Identity";

		private const string NewObjectRole = "New-MailboxSearch?StartDate&EndDate&SourceMailboxes&Language";

		private const string RemoveObjectRole = "Remove-MailboxSearch?Identity";

		private const string SetObjectRole = "Get-MailboxSearch?Identity+Set-MailboxSearch?Identity";

		private const string StartSearchRole = "Start-MailboxSearch?Identity";

		private const string StopSearchRole = "Stop-MailboxSearch?Identity";
	}
}
