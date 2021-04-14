using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderSyncUpgrade
	{
		public FolderSyncUpgrade(StoreObjectId rootIdIn)
		{
			this.rootId = rootIdIn;
		}

		private FolderSyncUpgrade()
		{
		}

		public Dictionary<string, StoreObjectId> Upgrade(FolderHierarchySync folderHierarchySyncIn, SyncState syncState, Dictionary<string, FolderNode> nodesOldInfo, out Dictionary<string, StoreObjectType> contentTypeTable)
		{
			contentTypeTable = new Dictionary<string, StoreObjectType>();
			this.syncState = syncState;
			this.folderHierarchySync = folderHierarchySyncIn;
			Dictionary<string, StoreObjectId> result = this.ProcessCommand(nodesOldInfo, contentTypeTable);
			this.folderHierarchySync.AcknowledgeServerOperations();
			return result;
		}

		private static string ConvertByteArrayToHex(byte[] id)
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			for (int i = 0; i < id.Length; i++)
			{
				stringBuilder.Append(id[i].ToString("x2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private static string GetOldIdFromNewId(byte[] id)
		{
			string text = FolderSyncUpgrade.ConvertByteArrayToHex(id);
			if (text.Length < 48)
			{
				return string.Empty;
			}
			return text.Substring(text.Length - 48, 44);
		}

		private static void StoreType(FolderNode foldernode, StoreObjectId storeObjectId, Dictionary<string, StoreObjectType> contentTypeTable)
		{
			if (foldernode.ContentClass.CompareTo(FolderSyncUpgrade.folderType) == 0)
			{
				contentTypeTable[foldernode.ServerId] = StoreObjectType.Folder;
				return;
			}
			if (foldernode.ContentClass.CompareTo(FolderSyncUpgrade.emailType) == 0)
			{
				contentTypeTable[foldernode.ServerId] = StoreObjectType.Folder;
				return;
			}
			if (foldernode.ContentClass.CompareTo(FolderSyncUpgrade.calendarType) == 0)
			{
				contentTypeTable[foldernode.ServerId] = StoreObjectType.CalendarFolder;
				return;
			}
			if (foldernode.ContentClass.CompareTo(FolderSyncUpgrade.taskType) == 0)
			{
				contentTypeTable[foldernode.ServerId] = StoreObjectType.TasksFolder;
				return;
			}
			if (foldernode.ContentClass.CompareTo(FolderSyncUpgrade.contactType) == 0)
			{
				contentTypeTable[foldernode.ServerId] = StoreObjectType.ContactsFolder;
				return;
			}
			contentTypeTable[foldernode.ServerId] = StoreObjectType.Unknown;
		}

		private string DepadNewId(string newId)
		{
			if (newId.Length != 44)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(newId, 0, 32, 40);
			stringBuilder.Append("-");
			int num = 38;
			while (num < 44 && newId[num] == '0')
			{
				num++;
			}
			stringBuilder.Append(newId.Substring(num));
			return stringBuilder.ToString();
		}

		private string PadOldId(string oldId)
		{
			string result;
			if (oldId.CompareTo("0") == 0)
			{
				result = FolderSyncUpgrade.GetOldIdFromNewId(this.rootId.ProviderLevelItemId);
			}
			else
			{
				int num = oldId.IndexOf("-");
				result = oldId;
				if (num >= 0)
				{
					result = oldId.Substring(0, num) + "0000000000000000000000000000000000000000".Substring(0, 44 - oldId.Length + 1) + oldId.Substring(num + 1, oldId.Length - num - 1);
				}
			}
			return result;
		}

		private Dictionary<string, StoreObjectId> ProcessCommand(Dictionary<string, FolderNode> folders, Dictionary<string, StoreObjectType> contentTypeTable)
		{
			HierarchySyncOperations hierarchySyncOperations = this.folderHierarchySync.EnumerateServerOperations();
			if (hierarchySyncOperations == null)
			{
				throw new ApplicationException("EnumerateServerOperations returned null!");
			}
			this.serverManifest = ((GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderManifestEntry>)this.syncState[SyncStateProp.CurServerManifest]).Data;
			if (this.serverManifest == null)
			{
				throw new ApplicationException("Server Manifest returned null!");
			}
			Dictionary<string, StoreObjectId> dictionary = new Dictionary<string, StoreObjectId>();
			if (folders != null)
			{
				for (int i = 0; i < hierarchySyncOperations.Count; i++)
				{
					HierarchySyncOperation hierarchySyncOperation = hierarchySyncOperations[i];
					using (Folder folder = hierarchySyncOperation.GetFolder())
					{
						string oldIdFromNewId = FolderSyncUpgrade.GetOldIdFromNewId(folder.Id.ObjectId.ProviderLevelItemId);
						if (oldIdFromNewId == null)
						{
							throw new ApplicationException("The new Id is invalid!");
						}
						FolderNode folderNode;
						if (folders.TryGetValue(oldIdFromNewId, out folderNode))
						{
							FolderSyncUpgrade.StoreType(folderNode, folder.Id.ObjectId, contentTypeTable);
							folders.Remove(oldIdFromNewId);
							dictionary.Add(folderNode.ServerId, folder.Id.ObjectId);
							FolderManifestEntry folderManifestEntry = this.serverManifest[folder.Id.ObjectId];
							if (FolderSyncUpgrade.GetOldIdFromNewId(folderManifestEntry.ParentId.ProviderLevelItemId).CompareTo(this.PadOldId(folderNode.ParentId)) != 0 || folder.DisplayName.CompareTo(folderNode.DisplayName) != 0)
							{
								folderManifestEntry.ChangeKey = new byte[]
								{
									1
								};
							}
						}
						else
						{
							if (FolderSyncUpgrade.GetOldIdFromNewId(folder.Id.ObjectId.ProviderLevelItemId) == null)
							{
								throw new ApplicationException("The new Id is invalid!");
							}
							string key = this.DepadNewId(FolderSyncUpgrade.GetOldIdFromNewId(folder.Id.ObjectId.ProviderLevelItemId));
							dictionary.Add(key, folder.Id.ObjectId);
							if (folder.ClassName.Equals("IPF.Appointment"))
							{
								contentTypeTable[key] = StoreObjectType.CalendarFolder;
							}
							else if (folder.ClassName.Equals("IPF.Contact"))
							{
								contentTypeTable[key] = StoreObjectType.ContactsFolder;
							}
							else
							{
								contentTypeTable[key] = StoreObjectType.Folder;
							}
							this.serverManifest.Remove(folder.Id.ObjectId);
						}
					}
				}
				IEnumerator enumerator = folders.GetEnumerator();
				enumerator.Reset();
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					FolderNode value = ((KeyValuePair<string, FolderNode>)obj).Value;
					UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
					byte[] bytes = unicodeEncoding.GetBytes(this.PadOldId(value.ServerId));
					StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(bytes, StoreObjectType.Mailbox);
					dictionary.Add(value.ServerId, storeObjectId);
					FolderSyncUpgrade.StoreType(value, storeObjectId, contentTypeTable);
					FolderManifestEntry folderManifestEntry2 = new FolderManifestEntry(storeObjectId);
					folderManifestEntry2.ChangeKey = new byte[]
					{
						1
					};
					folderManifestEntry2.ChangeType = ChangeType.Add;
					StoreObjectId storeObjectId2;
					dictionary.TryGetValue(value.ParentId, out storeObjectId2);
					if (storeObjectId2 == null)
					{
						if (value.ParentId.CompareTo("0") == 0)
						{
							folderManifestEntry2.ParentId = this.rootId;
						}
						else
						{
							folderManifestEntry2.ParentId = StoreObjectId.FromProviderSpecificId(unicodeEncoding.GetBytes(this.PadOldId(value.ParentId)));
						}
					}
					else
					{
						folderManifestEntry2.ParentId = storeObjectId2;
					}
					this.serverManifest[storeObjectId] = folderManifestEntry2;
				}
			}
			else
			{
				for (int j = 0; j < hierarchySyncOperations.Count; j++)
				{
					HierarchySyncOperation hierarchySyncOperation2 = hierarchySyncOperations[j];
					using (Folder folder2 = hierarchySyncOperation2.GetFolder())
					{
						string oldIdFromNewId2 = FolderSyncUpgrade.GetOldIdFromNewId(folder2.Id.ObjectId.ProviderLevelItemId);
						if (oldIdFromNewId2 == null)
						{
							throw new ApplicationException("The new Id is invalid!");
						}
						string key2 = this.DepadNewId(oldIdFromNewId2);
						dictionary.Add(key2, folder2.Id.ObjectId);
						if (folder2.ClassName.Equals("IPF.Appointment"))
						{
							contentTypeTable[key2] = StoreObjectType.CalendarFolder;
						}
						else if (folder2.ClassName.Equals("IPF.Contact"))
						{
							contentTypeTable[key2] = StoreObjectType.ContactsFolder;
						}
						else
						{
							contentTypeTable[key2] = StoreObjectType.Folder;
						}
					}
				}
			}
			return dictionary;
		}

		private const int NumberOfDigitsOfDavId = 44;

		private const int NumberOfTrailingDigits = 4;

		private const string ZeroString = "0000000000000000000000000000000000000000";

		private static string calendarType = "calendarfolder";

		private static string contactType = "contactfolder";

		private static string emailType = "mailfolder";

		private static string folderType = "folder";

		private static string taskType = "taskfolder";

		private FolderHierarchySync folderHierarchySync;

		private StoreObjectId rootId;

		private Dictionary<StoreObjectId, FolderManifestEntry> serverManifest;

		private SyncState syncState;
	}
}
