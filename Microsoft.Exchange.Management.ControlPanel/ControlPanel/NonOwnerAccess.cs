using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class NonOwnerAccess : DataSourceService, INonOwnerAccess, IGetListService<NonOwnerAccessFilter, NonOwnerAccessResultRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-MailboxAuditLog?ResultSize&StartDate&EndDate&Mailboxes&LogonTypes&ExternalAccess@R:Organization")]
		public PowerShellResults<NonOwnerAccessResultRow> GetList(NonOwnerAccessFilter filter, SortOptions sort)
		{
			string logonTypes = filter.LogonTypes;
			string logonTypes2;
			if ((logonTypes2 = filter.LogonTypes) != null)
			{
				if (!(logonTypes2 == "AllNonOwners"))
				{
					if (!(logonTypes2 == "OutsideUsers"))
					{
						if (!(logonTypes2 == "InternalUsers"))
						{
							if (logonTypes2 == "NonDelegateUsers")
							{
								filter.LogonTypes = "Admin";
								filter.ExternalAccess = new bool?(false);
							}
						}
						else
						{
							filter.LogonTypes = "Admin,Delegate";
							filter.ExternalAccess = new bool?(false);
						}
					}
					else
					{
						filter.LogonTypes = null;
						filter.ExternalAccess = new bool?(true);
					}
				}
				else
				{
					filter.LogonTypes = "Admin,Delegate";
				}
			}
			PowerShellResults<NonOwnerAccessResultRow> list = base.GetList<NonOwnerAccessResultRow, NonOwnerAccessFilter>("Search-MailboxAuditLog", filter, sort);
			if (list.Succeeded)
			{
				PowerShellResults<NonOwnerAccessResultRow> powerShellResults = new PowerShellResults<NonOwnerAccessResultRow>();
				int num = list.Output.Length;
				NonOwnerAccessResultRow[] array = new NonOwnerAccessResultRow[num];
				for (int i = 0; i < num; i++)
				{
					Identity id = AuditHelper.CreateNonOwnerAccessIdentity(list.Output[i].Mailbox, filter.StartDate, filter.EndDate, logonTypes);
					array[i] = new NonOwnerAccessResultRow(id, list.Output[i].NonOwnerAccessResult);
				}
				powerShellResults.Output = array;
				return powerShellResults;
			}
			return list;
		}

		internal const string NoStart = "NoStart";

		internal const string NoEnd = "NoEnd";

		internal const string GetCmdlet = "Search-MailboxAuditLog";

		internal const string ReadScope = "@R:Organization";

		private const string GetListRole = "Search-MailboxAuditLog?ResultSize&StartDate&EndDate&Mailboxes&LogonTypes&ExternalAccess@R:Organization";
	}
}
