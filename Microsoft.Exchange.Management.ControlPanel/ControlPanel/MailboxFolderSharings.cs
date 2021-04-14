using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MailboxFolderSharings : DataSourceService, IMailboxFolderSharings, IGetListService<MailboxFolderPermissionFilter, MailboxFolderPermissionRow>, IEditObjectService<UserMailboxFolderPermission, SetUserMailboxFolderPermission>, IGetObjectService<UserMailboxFolderPermission>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxFolderPermission?Identity@R:Self")]
		public PowerShellResults<MailboxFolderPermissionRow> GetList(MailboxFolderPermissionFilter filter, SortOptions sort)
		{
			Identity originalFolderId = filter.Identity;
			filter.Identity = (Identity)filter.Identity.ToMailboxFolderIdParameter();
			PowerShellResults<MailboxFolderPermissionRow> list = base.GetList<MailboxFolderPermissionRow, MailboxFolderPermissionFilter>("Get-MailboxFolderPermission", filter, sort);
			list.Output = Array.FindAll<MailboxFolderPermissionRow>(list.Output, (MailboxFolderPermissionRow x) => !x.IsAnonymousOrDefault);
			Array.ForEach<MailboxFolderPermissionRow>(list.Output, delegate(MailboxFolderPermissionRow x)
			{
				x.MailboxFolderId = originalFolderId;
			});
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-MailboxFolderPermission?Identity&User@W:Self")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			Identity identity = identities.IsNullOrEmpty() ? null : ((Identity)((MailboxFolderPermissionIdentity)identities[0]).MailboxFolderId.ToMailboxFolderIdParameter());
			return base.RemoveObjects("Remove-MailboxFolderPermission", identity, identities, "User", parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxFolderPermission?Identity&User@R:Self")]
		public PowerShellResults<UserMailboxFolderPermission> GetObject(Identity identity)
		{
			MailboxFolderPermissionIdentity mailboxFolderPermissionIdentity = identity.ToMailboxFolderPermissionIdentity();
			PSCommand pscommand = new PSCommand().AddCommand("Get-MailboxFolderPermission");
			pscommand.AddParameter("User", mailboxFolderPermissionIdentity.RawIdentity);
			return base.GetObject<UserMailboxFolderPermission>(pscommand, (Identity)mailboxFolderPermissionIdentity.MailboxFolderId.ToMailboxFolderIdParameter());
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxFolderPermission?Identity&User@R:Self+Set-MailboxFolderPermission?Identity&User@W:Self")]
		public PowerShellResults<UserMailboxFolderPermission> SetObject(Identity identity, SetUserMailboxFolderPermission properties)
		{
			properties.FaultIfNull();
			MailboxFolderPermissionIdentity mailboxFolderPermissionIdentity = identity.ToMailboxFolderPermissionIdentity();
			properties.User = mailboxFolderPermissionIdentity.RawIdentity;
			properties.ReturnObjectType = ReturnObjectTypes.PartialForList;
			return base.SetObject<UserMailboxFolderPermission, SetUserMailboxFolderPermission>("Set-MailboxFolderPermission", (Identity)mailboxFolderPermissionIdentity.MailboxFolderId.ToMailboxFolderIdParameter(), properties);
		}

		internal const string GetCmdlet = "Get-MailboxFolderPermission";

		internal const string SetCmdlet = "Set-MailboxFolderPermission";

		internal const string RemoveCmdlet = "Remove-MailboxFolderPermission";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetListRole = "Get-MailboxFolderPermission?Identity@R:Self";

		private const string GetObjectRole = "Get-MailboxFolderPermission?Identity&User@R:Self";

		private const string SetObjectRole = "Get-MailboxFolderPermission?Identity&User@R:Self+Set-MailboxFolderPermission?Identity&User@W:Self";

		private const string RemoveObjectRole = "Remove-MailboxFolderPermission?Identity&User@W:Self";
	}
}
