using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ElcArchiveStoreDataProvider : EwsStoreDataProvider
	{
		public ElcArchiveStoreDataProvider(IExchangePrincipal primaryExchangePrincipal) : base(new LazilyInitialized<IExchangePrincipal>(() => primaryExchangePrincipal))
		{
			IMailboxInfo archiveMailbox = primaryExchangePrincipal.GetArchiveMailbox();
			if (archiveMailbox != null && archiveMailbox.IsRemote)
			{
				throw new NotImplementedException("Cross premise remote archive is not supported yet");
			}
			base.LogonType = new SpecialLogonType?(SpecialLogonType.SystemService);
			base.BudgetType = OpenAsAdminOrSystemServiceBudgetTypeType.RunAsBackgroundLoad;
		}

		public bool CreateOrUpdateUserConfiguration(byte[] xmlData, FolderId folderId, string configName)
		{
			UserConfiguration userConfig = base.InvokeServiceCall<UserConfiguration>(() => UserConfiguration.Bind(this.Service, configName, folderId, 4));
			if (userConfig != null)
			{
				userConfig.XmlData = xmlData;
				base.InvokeServiceCall(delegate()
				{
					userConfig.Update();
				});
			}
			else
			{
				userConfig = new UserConfiguration(base.Service);
				userConfig.XmlData = xmlData;
				base.InvokeServiceCall(delegate()
				{
					userConfig.Save(configName, folderId);
				});
			}
			return true;
		}

		public void DeleteUserConfiguration(FolderId folderId, string configName)
		{
			UserConfiguration userConfig = base.InvokeServiceCall<UserConfiguration>(() => UserConfiguration.Bind(this.Service, configName, folderId, 4));
			if (userConfig != null)
			{
				base.InvokeServiceCall(delegate()
				{
					userConfig.Delete();
				});
			}
		}

		public bool SaveUserConfiguration(byte[] xmlData, string configName, out Exception ex)
		{
			ex = null;
			bool result = false;
			try
			{
				Folder orCreateFolder = base.GetOrCreateFolder(ClientStrings.Inbox, new FolderId(20));
				result = this.CreateOrUpdateUserConfiguration(xmlData, orCreateFolder.Id, configName);
			}
			catch (DataSourceOperationException ex2)
			{
				result = false;
				ex = ex2;
			}
			return result;
		}

		public Folder GetDefaultFolder(WellKnownFolderName folderName, out Exception ex)
		{
			Folder result = null;
			ex = null;
			try
			{
				result = base.InvokeServiceCall<Folder>(() => Folder.Bind(this.Service, folderName));
			}
			catch (DataSourceOperationException ex2)
			{
				ex = ex2;
			}
			return result;
		}

		public Folder GetFolder(WellKnownFolderName folderName, List<PropertyDefinitionBase> properties)
		{
			PropertySet retentionPropertySet = new PropertySet(0, properties);
			return base.InvokeServiceCall<Folder>(() => Folder.Bind(this.Service, folderName, retentionPropertySet));
		}

		public Folder GetCreateFolder(string displayName, FolderId parentFolder)
		{
			return base.GetOrCreateFolder(displayName, parentFolder);
		}

		public void SaveFolder(Folder folder, FolderId parentId)
		{
			base.InvokeServiceCall(delegate()
			{
				folder.Save(parentId);
			});
		}

		public void UpdateFolder(Folder folder)
		{
			base.InvokeServiceCall(delegate()
			{
				folder.Update();
			});
		}

		public bool MoveItems(List<ItemId> itemIds, FolderId destinationFolderIdInArchive, out Exception ex)
		{
			ex = null;
			bool result;
			try
			{
				base.InvokeServiceCall(delegate()
				{
					this.Service.MoveItems(itemIds, destinationFolderIdInArchive);
				});
				result = true;
			}
			catch (DataSourceOperationException ex2)
			{
				ex = ex2;
				result = false;
			}
			return result;
		}

		public IEnumerable<Folder> GetFolderHierarchy(int pageSize, List<PropertyDefinitionBase> properties, WellKnownFolderName rootFolder)
		{
			FolderView folderView = new FolderView(pageSize);
			folderView.Traversal = 1;
			folderView.PropertySet = new PropertySet(0, properties.ToArray());
			for (;;)
			{
				FindFoldersResults findFolderResults = base.InvokeServiceCall<FindFoldersResults>(() => this.Service.FindFolders(rootFolder, folderView));
				IEnumerable<Folder> folders = findFolderResults.Folders;
				foreach (Folder folder in folders)
				{
					yield return folder;
				}
				if (!findFolderResults.MoreAvailable)
				{
					break;
				}
				folderView.Offset = findFolderResults.NextPageOffset.Value;
			}
			yield break;
		}

		public void DeleteUserConfiguration(string configName, out Exception ex)
		{
			ex = null;
			try
			{
				Folder orCreateFolder = base.GetOrCreateFolder(ClientStrings.Inbox, new FolderId(20));
				this.DeleteUserConfiguration(orCreateFolder.Id, configName);
			}
			catch (DataSourceOperationException ex2)
			{
				ex = ex2;
			}
		}
	}
}
