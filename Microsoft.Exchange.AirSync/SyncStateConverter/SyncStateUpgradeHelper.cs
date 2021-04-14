using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	internal class SyncStateUpgradeHelper
	{
		internal SyncStateUpgradeHelper(MailboxSession mailboxSessionIn, SyncStateStorage syncStateStorageIn)
		{
			this.maxFolderSeen = 1;
			this.needsParentId = new HashSet<TiSyncUpgrade>();
			this.mailboxSession = mailboxSessionIn;
			this.syncStateStorage = syncStateStorageIn;
		}

		internal int MaxFolderSeen
		{
			get
			{
				return this.maxFolderSeen;
			}
			set
			{
				this.maxFolderSeen = value;
			}
		}

		public bool UpgradeSyncState(Dictionary<string, StoreObjectId> mapping, Dictionary<string, StoreObjectType> contentTypeTable, HashSet<string> folderList, MailboxUtility box, StoreObjectId parentStoreObjectId)
		{
			using (SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(Marshal.SizeOf(typeof(CommonInfo))))
			{
				foreach (string text in mapping.Keys)
				{
					StoreObjectId storeId = mapping[text];
					if (folderList.Contains(text))
					{
						MemoryStream memoryStream = null;
						try
						{
							memoryStream = box.GetSyncState(parentStoreObjectId, text);
							if (memoryStream == null)
							{
								continue;
							}
							if (!SyncStateUpgradeHelper.MakeCppCall(safeHGlobalHandle, memoryStream.GetBuffer(), (int)memoryStream.Length, contentTypeTable, text))
							{
								return false;
							}
						}
						finally
						{
							if (memoryStream != null)
							{
								MailboxUtility.ReclaimStream(memoryStream);
							}
						}
						CommonInfo commonInfo = (CommonInfo)Marshal.PtrToStructure(safeHGlobalHandle.DangerousGetHandle(), typeof(CommonInfo));
						if (!this.CopyAndUpgradeCommon(commonInfo, storeId, text, contentTypeTable[text], safeHGlobalHandle))
						{
							return false;
						}
					}
				}
				this.maxFolderSeen++;
				foreach (TiSyncUpgrade tiSyncUpgrade in this.needsParentId)
				{
					string shortParentId = string.Empty + this.maxFolderSeen;
					tiSyncUpgrade.UpdateMappingWithParent(shortParentId);
					this.maxFolderSeen++;
				}
			}
			return true;
		}

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Email_upgrade(SafeHGlobalHandle bFile, uint cbSize, SafeHGlobalHandle commonInfo);

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Task_upgrade(SafeHGlobalHandle bFile, uint cbSize, SafeHGlobalHandle commonInfo);

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Con_upgrade(SafeHGlobalHandle bFile, uint cbSize, SafeHGlobalHandle commonInfo);

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Cal_upgrade(SafeHGlobalHandle bFile, uint cbSize, SafeHGlobalHandle commonInfo);

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Commoninfo_cleanup(SafeHGlobalHandle commonInfo);

		private static bool MakeCppCall(SafeHGlobalHandle pciInfo, byte[] bytes, int length, Dictionary<string, StoreObjectType> contentTypeTable, string serverId)
		{
			int num = 0;
			using (SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(length))
			{
				Marshal.Copy(bytes, 0, safeHGlobalHandle.DangerousGetHandle(), length);
				switch (contentTypeTable[serverId])
				{
				case StoreObjectType.Folder:
					num = SyncStateUpgradeHelper.Email_upgrade(safeHGlobalHandle, (uint)length, pciInfo);
					break;
				case StoreObjectType.CalendarFolder:
					num = SyncStateUpgradeHelper.Cal_upgrade(safeHGlobalHandle, (uint)length, pciInfo);
					break;
				case StoreObjectType.ContactsFolder:
					num = SyncStateUpgradeHelper.Con_upgrade(safeHGlobalHandle, (uint)length, pciInfo);
					break;
				case StoreObjectType.TasksFolder:
					num = SyncStateUpgradeHelper.Task_upgrade(safeHGlobalHandle, (uint)length, pciInfo);
					break;
				default:
					return false;
				}
			}
			return num == 0;
		}

		private bool CopyAndUpgradeCommon(CommonInfo commonInfo, StoreObjectId storeId, string parentServerId, StoreObjectType type, SafeHGlobalHandle pciInfo)
		{
			Dictionary<string, string> dictionary = null;
			List<TagNode> list = null;
			Dictionary<string, CommonNode> dictionary2 = new Dictionary<string, CommonNode>((int)commonInfo.NumCommonNodes);
			string text = null;
			long num = 1L;
			TiSyncUpgrade tiSyncUpgrade = new TiSyncUpgrade(parentServerId, this.mailboxSession, this.syncStateStorage);
			try
			{
				dictionary = new Dictionary<string, string>((int)commonInfo.NumMapping);
				if (commonInfo.NumMapping > 0U)
				{
					MappingNodeStruct mappingNodeStruct = (MappingNodeStruct)Marshal.PtrToStructure(commonInfo.Mappingnodes, typeof(MappingNodeStruct));
					int num2 = mappingNodeStruct.ShortId.LastIndexOf(':');
					if (num2 != -1)
					{
						text = mappingNodeStruct.ShortId.Substring(0, num2);
						uint num3;
						if (uint.TryParse(text, out num3))
						{
							this.maxFolderSeen = ((this.maxFolderSeen > (int)num3) ? this.maxFolderSeen : ((int)num3));
						}
						else
						{
							this.needsParentId.Add(tiSyncUpgrade);
						}
					}
					else
					{
						this.needsParentId.Add(tiSyncUpgrade);
					}
					for (uint num4 = 0U; num4 < commonInfo.NumMapping; num4 += 1U)
					{
						int num5 = mappingNodeStruct.LongId.LastIndexOf(':');
						string key;
						if (num5 != -1)
						{
							key = mappingNodeStruct.LongId.Substring(num5 + 1);
						}
						else
						{
							key = mappingNodeStruct.LongId;
						}
						dictionary.Add(key, mappingNodeStruct.ShortId);
						num2 = mappingNodeStruct.ShortId.LastIndexOf(':');
						if (num2 != -1)
						{
							string s = mappingNodeStruct.ShortId.Substring(num2 + 1);
							uint num6;
							if (uint.TryParse(s, out num6))
							{
								num = ((num > (long)((ulong)num6)) ? num : ((long)((ulong)num6)));
							}
						}
						if (num4 < commonInfo.NumMapping - 1U)
						{
							mappingNodeStruct = (MappingNodeStruct)Marshal.PtrToStructure(mappingNodeStruct.Next, typeof(MappingNodeStruct));
						}
					}
				}
				else
				{
					this.needsParentId.Add(tiSyncUpgrade);
				}
				CommonNodeStruct commonNodeStruct;
				if (commonInfo.NumCommonNodes > 0U)
				{
					commonNodeStruct = (CommonNodeStruct)Marshal.PtrToStructure(commonInfo.Nodes, typeof(CommonNodeStruct));
				}
				else
				{
					commonNodeStruct = default(CommonNodeStruct);
				}
				for (uint num7 = 0U; num7 < commonInfo.NumCommonNodes; num7 += 1U)
				{
					if (commonNodeStruct.SentToClient == 1)
					{
						if (!dictionary.ContainsKey(commonNodeStruct.ServerId))
						{
							dictionary.Add(commonNodeStruct.ServerId, commonNodeStruct.ServerId);
						}
						ExDateTime endTimeIn = (commonNodeStruct.EndTime == 0L) ? ExDateTime.MinValue : ExDateTime.FromFileTimeUtc(commonNodeStruct.EndTime);
						dictionary2.Add(commonNodeStruct.ServerId, new CommonNode(commonNodeStruct.ServerId, commonNodeStruct.VersionId, commonNodeStruct.SentToClient, commonNodeStruct.IsEmail, commonNodeStruct.Read, commonNodeStruct.IsCalendar, endTimeIn));
					}
					if (num7 < commonInfo.NumCommonNodes - 1U)
					{
						commonNodeStruct = (CommonNodeStruct)Marshal.PtrToStructure(commonNodeStruct.Next, typeof(CommonNodeStruct));
					}
				}
				list = new List<TagNode>((int)commonInfo.NumSupportedTags);
				TagNodeStruct tagNodeStruct = default(TagNodeStruct);
				if (commonInfo.NumSupportedTags > 0U)
				{
					tagNodeStruct = (TagNodeStruct)Marshal.PtrToStructure(commonInfo.Tagnodes, typeof(TagNodeStruct));
				}
				for (uint num8 = 0U; num8 < commonInfo.NumSupportedTags; num8 += 1U)
				{
					list.Add(new TagNode(tagNodeStruct.NameSpace, tagNodeStruct.Tag));
					if (num8 < commonInfo.NumSupportedTags - 1U)
					{
						tagNodeStruct = (TagNodeStruct)Marshal.PtrToStructure(tagNodeStruct.Next, typeof(TagNodeStruct));
					}
				}
			}
			finally
			{
				SyncStateUpgradeHelper.Commoninfo_cleanup(pciInfo);
			}
			uint version;
			switch (commonInfo.Version)
			{
			case 65542U:
				version = 20U;
				break;
			case 65543U:
				version = 21U;
				break;
			case 65544U:
				version = 25U;
				break;
			default:
				version = 10U;
				break;
			}
			return tiSyncUpgrade.Upgrade(storeId, dictionary2, dictionary, list, type, new string(commonInfo.SyncKey), text, (int)num, version);
		}

		private int maxFolderSeen;

		private MailboxSession mailboxSession;

		private SyncStateStorage syncStateStorage;

		private HashSet<TiSyncUpgrade> needsParentId;
	}
}
