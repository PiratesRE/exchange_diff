using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class RetentionPolicyTags : DataSourceService, IRetentionPolicyTags, IGetListService<AllAssociatedRPTsFilter, RetentionPolicyTagRow>, IGetObjectService<ViewRetentionPolicyTagRow>, INewObjectService<RetentionPolicyTagRow, AddRetentionPolicyTag>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RetentionPolicyTag?Types&Mailbox&OptionalInMailbox@C:OrganizationConfig")]
		public PowerShellResults<RetentionPolicyTagRow> GetList(AllAssociatedRPTsFilter filter, SortOptions sort)
		{
			filter.IncludeDefaultTags = true;
			PowerShellResults<RetentionPolicyTagRow> list = base.GetList<RetentionPolicyTagRow, AllAssociatedRPTsFilter>("Get-RetentionPolicyTag", filter, null);
			PowerShellResults<RetentionPolicyTagRow> powerShellResults = list.MergeErrors<RetentionPolicyTagRow>(base.GetList<RetentionPolicyTagRow, OptInRPTsFilter>("Get-RetentionPolicyTag", new OptInRPTsFilter(), null));
			if (list.Succeeded && RbacPrincipal.Current.ExecutingUserId != null)
			{
				Accounts accounts = new Accounts();
				if (!RetentionUtils.UserHasArchive(accounts.GetRecipientObject(null)))
				{
					List<RetentionPolicyTagRow> list2 = new List<RetentionPolicyTagRow>();
					foreach (RetentionPolicyTagRow retentionPolicyTagRow in list.Output)
					{
						if (retentionPolicyTagRow.RetentionPolicyTag.RetentionAction != RetentionActionType.MoveToArchive)
						{
							list2.Add(retentionPolicyTagRow);
						}
					}
					list.Output = list2.ToArray();
				}
			}
			if (list.Succeeded && powerShellResults.Succeeded)
			{
				foreach (RetentionPolicyTagRow retentionPolicyTagRow2 in list.Output.Intersect(powerShellResults.Output))
				{
					retentionPolicyTagRow2.OptionalTag = true;
				}
				if (sort != null)
				{
					sort.PropertyName = RetentionPolicyTagBaseRow.GetSortProperty(sort.PropertyName);
					Func<RetentionPolicyTagRow[], RetentionPolicyTagRow[]> sortFunction = sort.GetSortFunction<RetentionPolicyTagRow>();
					if (sortFunction != null)
					{
						list.Output = sortFunction(list.Output);
					}
				}
			}
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RetentionPolicyTag?Identity@C:OrganizationConfig")]
		public PowerShellResults<ViewRetentionPolicyTagRow> GetObject(Identity identity)
		{
			PowerShellResults<ViewRetentionPolicyTagRow> @object = base.GetObject<ViewRetentionPolicyTagRow>("Get-RetentionPolicyTag", identity);
			PowerShellResults<ViewRetentionPolicyTagRow> powerShellResults = @object.MergeErrors<ViewRetentionPolicyTagRow>(base.GetList<ViewRetentionPolicyTagRow, OptInRPTsFilter>("Get-RetentionPolicyTag", new OptInRPTsFilter(), null));
			if (@object.SucceededWithValue && powerShellResults.Succeeded && powerShellResults.Output.Contains(@object.Value))
			{
				@object.Value.OptionalTag = true;
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-RetentionPolicyTag?Mailbox&OptionalInMailbox@C:OrganizationConfig")]
		public PowerShellResults<RetentionPolicyTagRow> NewObject(AddRetentionPolicyTag properties)
		{
			properties.FaultIfNull();
			PowerShellResults<RetentionPolicyTagRow> powerShellResults = new PowerShellResults<RetentionPolicyTagRow>();
			PowerShellResults<RetentionPolicyTagRow> powerShellResults2 = powerShellResults.MergeErrors<RetentionPolicyTagRow>(base.GetList<RetentionPolicyTagRow, AllAssociatedRPTsFilter>("Get-RetentionPolicyTag", new AllAssociatedRPTsFilter(), null));
			if (powerShellResults2.Failed)
			{
				return powerShellResults;
			}
			PSCommand pscommand = new PSCommand().AddCommand("Set-RetentionPolicyTag");
			pscommand.AddParameter("Mailbox", RbacPrincipal.Current.ExecutingUserId);
			IEnumerable<Identity> enumerable = from x in powerShellResults2.Output
			select x.Identity;
			if (properties.OptionalInMailbox != null)
			{
				enumerable = enumerable.Union(properties.OptionalInMailbox);
			}
			pscommand.AddParameter("OptionalInMailBox", enumerable);
			powerShellResults.MergeErrors(base.Invoke(pscommand));
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-RetentionPolicyTag?Mailbox&OptionalInMailbox@C:OrganizationConfig")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PowerShellResults powerShellResults = new PowerShellResults();
			PowerShellResults<RetentionPolicyTagRow> powerShellResults2 = powerShellResults.MergeErrors<RetentionPolicyTagRow>(base.GetList<RetentionPolicyTagRow, AllAssociatedRPTsFilter>("Get-RetentionPolicyTag", new AllAssociatedRPTsFilter(), null));
			if (powerShellResults2.Failed)
			{
				return powerShellResults;
			}
			PSCommand pscommand = new PSCommand().AddCommand("Set-RetentionPolicyTag");
			pscommand.AddParameter("Mailbox", RbacPrincipal.Current.ExecutingUserId);
			pscommand.AddParameter("OptionalInMailBox", (from x in powerShellResults2.Output
			select x.Identity.RawIdentity).Except(from i in identities
			select i.RawIdentity));
			powerShellResults.MergeErrors(base.Invoke(pscommand));
			return powerShellResults;
		}

		private const string GetListRole = "Get-RetentionPolicyTag?Types&Mailbox&OptionalInMailbox@C:OrganizationConfig";

		private const string GetObjectRole = "Get-RetentionPolicyTag?Identity@C:OrganizationConfig";

		private const string SetObjectRole = "Set-RetentionPolicyTag?Mailbox&OptionalInMailbox@C:OrganizationConfig";
	}
}
