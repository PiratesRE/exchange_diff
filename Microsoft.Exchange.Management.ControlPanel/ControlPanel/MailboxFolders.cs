using System;
using System.Collections;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MailboxFolders : DataSourceService, IMailboxFolders, IGetListService<MailboxFolderFilter, MailboxFolder>, INewObjectService<MailboxFolder, NewMailboxFolder>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxFolder?Recurse&MailFolderOnly&ResultSize@R:Self")]
		public PowerShellResults<MailboxFolder> GetList(MailboxFolderFilter filter, SortOptions sort)
		{
			PowerShellResults<MailboxFolder> list = base.GetList<MailboxFolder, MailboxFolderFilter>("Get-MailboxFolder", filter, sort, "Name");
			if (list.Succeeded)
			{
				list.Output = this.LinkFolders(list.Output, filter.FolderPickerType);
			}
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxFolder?Identity@R:Self")]
		public PowerShellResults<MailboxFolder> GetObject(Identity identity)
		{
			return base.GetObject<MailboxFolder>("Get-MailboxFolder", (Identity)identity.ToMailboxFolderIdParameter());
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-MailboxFolder?Name&Parent@W:Self")]
		public PowerShellResults<MailboxFolder> NewObject(NewMailboxFolder properties)
		{
			return base.NewObject<MailboxFolder, NewMailboxFolder>("New-MailboxFolder", properties);
		}

		private MailboxFolder[] LinkFolders(MailboxFolder[] rows, FolderPickerType folderPickerType)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < rows.Length; i++)
			{
				hashtable[rows[i].Folder.FolderStoreObjectId] = i;
			}
			for (int j = 0; j < rows.Length; j++)
			{
				MailboxFolder folder = rows[j].Folder;
				if (!(folder.DefaultFolderType == DefaultFolderType.CommunicatorHistory))
				{
					if (folderPickerType == FolderPickerType.VoiceMailFolderPicker)
					{
						if (folder.DefaultFolderType != DefaultFolderType.None && folder.DefaultFolderType != DefaultFolderType.Inbox && folder.DefaultFolderType != DefaultFolderType.Root)
						{
							goto IL_138;
						}
					}
					else
					{
						if (folderPickerType != FolderPickerType.RulesFolderPicker)
						{
							throw new NotSupportedException();
						}
						if (folder.DefaultFolderType == DefaultFolderType.Outbox)
						{
							goto IL_138;
						}
					}
					MailboxFolderId parentFolder = folder.ParentFolder;
					if (parentFolder != null && hashtable.Contains(parentFolder.StoreObjectId))
					{
						int num = (int)hashtable[parentFolder.StoreObjectId];
						rows[num].Children.Add(rows[j]);
					}
				}
				IL_138:;
			}
			for (int k = 0; k < rows.Length; k++)
			{
				if (rows[k].Folder.DefaultFolderType == DefaultFolderType.Root)
				{
					rows[k].Name = RbacPrincipal.Current.Name;
					rows[k].Children.Sort();
					return new MailboxFolder[]
					{
						rows[k]
					};
				}
			}
			return null;
		}

		private const string Noun = "MailboxFolder";

		internal const string GetCmdlet = "Get-MailboxFolder";

		internal const string NewCmdlet = "New-MailboxFolder";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetListRole = "Get-MailboxFolder?Recurse&MailFolderOnly&ResultSize@R:Self";

		private const string GetObjectRole = "Get-MailboxFolder?Identity@R:Self";

		private const string NewObjectRole = "New-MailboxFolder?Name&Parent@W:Self";

		internal static readonly MailboxFolders Instance = new MailboxFolders();
	}
}
