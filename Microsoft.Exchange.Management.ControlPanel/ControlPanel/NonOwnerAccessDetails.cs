using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class NonOwnerAccessDetails : DataSourceService, INonOwnerAccessDetails, IGetObjectService<NonOwnerAccessDetailRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Search-MailboxAuditLog?ResultSize&StartDate&EndDate&Mailboxes&LogonTypes&ExternalAccess@R:Organization")]
		public PowerShellResults<NonOwnerAccessDetailRow> GetObject(Identity identity)
		{
			NonOwnerAccessDetailsId nonOwnerAccessDetailsId = new NonOwnerAccessDetailsId(identity);
			NonOwnerAccessFilter nonOwnerAccessFilter = new NonOwnerAccessFilter();
			if (nonOwnerAccessDetailsId.StartDate != "NoStart")
			{
				nonOwnerAccessFilter.StartDate = nonOwnerAccessDetailsId.StartDate;
			}
			if (nonOwnerAccessDetailsId.EndDate != "NoEnd")
			{
				nonOwnerAccessFilter.EndDate = nonOwnerAccessDetailsId.EndDate;
			}
			string logonTypes;
			if (!nonOwnerAccessDetailsId.LogonTypes.IsNullOrBlank() && (logonTypes = nonOwnerAccessDetailsId.LogonTypes) != null)
			{
				if (!(logonTypes == "AllNonOwners"))
				{
					if (!(logonTypes == "OutsideUsers"))
					{
						if (!(logonTypes == "InternalUsers"))
						{
							if (logonTypes == "NonDelegateUsers")
							{
								nonOwnerAccessFilter.LogonTypes = "Admin";
								nonOwnerAccessFilter.ExternalAccess = new bool?(false);
							}
						}
						else
						{
							nonOwnerAccessFilter.LogonTypes = "Admin,Delegate";
							nonOwnerAccessFilter.ExternalAccess = new bool?(false);
						}
					}
					else
					{
						nonOwnerAccessFilter.LogonTypes = null;
						nonOwnerAccessFilter.ExternalAccess = new bool?(true);
					}
				}
				else
				{
					nonOwnerAccessFilter.LogonTypes = "Admin,Delegate";
				}
			}
			PSCommand pscommand = new PSCommand().AddCommand("Search-MailboxAuditLog").AddParameters(nonOwnerAccessFilter);
			pscommand.AddParameter("identity", nonOwnerAccessDetailsId.Object);
			pscommand.AddParameter("showDetails", true);
			pscommand.AddParameter("resultSize", 501);
			PowerShellResults<MailboxAuditLogEvent> powerShellResults = base.Invoke<MailboxAuditLogEvent>(pscommand);
			PowerShellResults<NonOwnerAccessDetailRow> powerShellResults2 = new PowerShellResults<NonOwnerAccessDetailRow>();
			if (powerShellResults.Succeeded)
			{
				if (powerShellResults.Output.Length == 0)
				{
					powerShellResults2.Warnings = new string[]
					{
						Strings.AuditLogsDeleted
					};
				}
				if (powerShellResults.Output.Length == 501)
				{
					powerShellResults2.Warnings = new string[]
					{
						Strings.TooManyAuditLogsInDetailsPane
					};
				}
				NonOwnerAccessDetailRow[] array = new NonOwnerAccessDetailRow[powerShellResults.Output.Length];
				for (int i = 0; i < powerShellResults.Output.Length; i++)
				{
					array[i] = new NonOwnerAccessDetailRow(identity, powerShellResults.Output[i]);
				}
				powerShellResults2.MergeOutput(array);
				return powerShellResults2;
			}
			powerShellResults2.MergeErrors<MailboxAuditLogEvent>(powerShellResults);
			return powerShellResults2;
		}

		internal const string ReadScope = "@R:Organization";

		private const string GetObjectRole = "Search-MailboxAuditLog?ResultSize&StartDate&EndDate&Mailboxes&LogonTypes&ExternalAccess@R:Organization";
	}
}
