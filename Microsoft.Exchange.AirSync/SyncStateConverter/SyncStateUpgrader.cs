using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	internal static class SyncStateUpgrader
	{
		public static SyncStateUpgradeResult CheckAndUpgradeSyncStates(MailboxSession mailboxSession, DeviceIdentity deviceIdentity)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			TiFolderSyncUpgrade tiFolderSyncUpgrade = new TiFolderSyncUpgrade();
			tiFolderSyncUpgrade.MailboxSession = mailboxSession;
			MailboxUtility mailboxUtility = new MailboxUtility();
			MailboxUtilityDeviceInfo mailboxUtilityDeviceInfo = null;
			MemoryStream memoryStream = null;
			try
			{
				mailboxUtility.MailboxSessionForUtility = mailboxSession;
				mailboxUtilityDeviceInfo = mailboxUtility.GetDevice(deviceIdentity);
				if (mailboxUtilityDeviceInfo == null)
				{
					AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.TiUpgradeTracer, null, "Failed to retrieve device info for: {0}", deviceIdentity);
					return SyncStateUpgradeResult.NoTiSyncState;
				}
				AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.TiUpgradeTracer, null, "Starting sync state upgrade for device: {0}", deviceIdentity);
				safeHGlobalHandle = NativeMethods.AllocHGlobal(Marshal.SizeOf(typeof(FolderInfo)));
				StoreObjectId storeObjectId = null;
				storeObjectId = mailboxUtilityDeviceInfo.StoreObjectId;
				HashSet<string> folderList = mailboxUtilityDeviceInfo.FolderList;
				FolderInfo folderInfo = default(FolderInfo);
				bool containsFoldersyncFile = false;
				memoryStream = mailboxUtility.GetSyncState(storeObjectId, "FolderSyncFile");
				if (memoryStream != null)
				{
					using (SafeHGlobalHandle safeHGlobalHandle2 = NativeMethods.AllocHGlobal((int)memoryStream.Length))
					{
						Marshal.Copy(memoryStream.GetBuffer(), 0, safeHGlobalHandle2.DangerousGetHandle(), (int)memoryStream.Length);
						int num = SyncStateUpgrader.Foldersync_upgrade(safeHGlobalHandle2, (uint)memoryStream.Length, safeHGlobalHandle);
						if (num != 0)
						{
							throw new AirSyncPermanentException(false);
						}
					}
					folderInfo = (FolderInfo)Marshal.PtrToStructure(safeHGlobalHandle.DangerousGetHandle(), typeof(FolderInfo));
					containsFoldersyncFile = true;
					MailboxUtility.ReclaimStream(memoryStream);
					memoryStream = null;
				}
				Dictionary<string, StoreObjectType> dictionary2;
				Dictionary<string, StoreObjectId> dictionary = SyncStateUpgrader.UpgradeFolderSyncHierarchySyncState(tiFolderSyncUpgrade, containsFoldersyncFile, folderInfo, deviceIdentity, out dictionary2);
				if (dictionary == null)
				{
					mailboxUtility.DeleteFolder(mailboxUtilityDeviceInfo.StoreObjectId, true);
				}
				else
				{
					SyncStateUpgradeHelper syncStateUpgradeHelper = new SyncStateUpgradeHelper(mailboxSession, tiFolderSyncUpgrade.SyncStateStorage);
					foreach (string key in dictionary2.Keys)
					{
						StoreObjectType storeObjectType = dictionary2[key];
						if (storeObjectType != StoreObjectType.Folder && storeObjectType != StoreObjectType.ContactsFolder && storeObjectType != StoreObjectType.CalendarFolder && storeObjectType != StoreObjectType.TasksFolder)
						{
							AirSyncDiagnostics.TraceDebug<StoreObjectType>(ExTraceGlobals.TiUpgradeTracer, null, "Removing unknown Ti folder of type {0}", storeObjectType);
							dictionary.Remove(key);
						}
					}
					if (!syncStateUpgradeHelper.UpgradeSyncState(dictionary, dictionary2, folderList, mailboxUtility, storeObjectId))
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.TiUpgradeTracer, null, "Failed to upgrade folders for {0}", mailboxUtilityDeviceInfo.DisplayName);
						mailboxUtility.DeleteFolder(mailboxUtilityDeviceInfo.StoreObjectId, true);
						return SyncStateUpgradeResult.UpgradeFailed;
					}
					tiFolderSyncUpgrade.UpdateLastFolderId(syncStateUpgradeHelper.MaxFolderSeen);
				}
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest(3236310333U);
			}
			catch (Exception arg)
			{
				if (mailboxUtilityDeviceInfo != null)
				{
					mailboxUtility.DeleteSyncStateStorage(new DeviceIdentity(mailboxUtilityDeviceInfo.DisplayName, mailboxUtilityDeviceInfo.ParentDisplayName, "AirSync"));
				}
				AirSyncDiagnostics.TraceDebug<DeviceIdentity, Exception>(ExTraceGlobals.TiUpgradeTracer, null, "Sync state upgrade failed for device: {0}\r\nException:\r\n{1}", deviceIdentity, arg);
				throw;
			}
			finally
			{
				if (memoryStream != null)
				{
					MailboxUtility.ReclaimStream(memoryStream);
					memoryStream = null;
				}
				if (mailboxUtilityDeviceInfo != null)
				{
					mailboxUtility.DeleteFolder(mailboxUtilityDeviceInfo.StoreObjectId, true);
				}
				if (tiFolderSyncUpgrade != null)
				{
					tiFolderSyncUpgrade.Close();
					tiFolderSyncUpgrade = null;
				}
				safeHGlobalHandle.Close();
			}
			AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.TiUpgradeTracer, null, "Finished sync state upgrade for device: {0}", deviceIdentity);
			return SyncStateUpgradeResult.UpgradeComplete;
		}

		private static string PadOldId(string oldId)
		{
			int num = oldId.IndexOf("-");
			string result = oldId;
			if (num >= 0)
			{
				result = oldId.Substring(0, num) + "0000000000000000000000000000000000000000".Substring(0, 44 - oldId.Length + 1) + oldId.Substring(num + 1, oldId.Length - num - 1);
			}
			return result;
		}

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Foldersync_cleanup(IntPtr linkedlist);

		[DllImport("AirsyncTiStateParser.dll")]
		private static extern int Foldersync_upgrade(SafeHGlobalHandle bFile, uint cbSize, SafeHGlobalHandle fInfo);

		private static Dictionary<string, StoreObjectId> UpgradeFolderSyncHierarchySyncState(TiFolderSyncUpgrade itemSyncUpgrade, bool containsFoldersyncFile, FolderInfo folderInfo, DeviceIdentity deviceIdentity, out Dictionary<string, StoreObjectType> contentTypeTable)
		{
			Dictionary<string, FolderNode> dictionary = new Dictionary<string, FolderNode>();
			if (containsFoldersyncFile)
			{
				try
				{
					FolderNodeStruct folderNodeStruct = (FolderNodeStruct)Marshal.PtrToStructure(folderInfo.Foldernodes, typeof(FolderNodeStruct));
					for (uint num = 0U; num < folderInfo.NumItems; num += 1U)
					{
						dictionary.Add(SyncStateUpgrader.PadOldId(folderNodeStruct.ServerID), new FolderNode(folderNodeStruct.ServerID, folderNodeStruct.DisplayName, folderNodeStruct.ParentID, folderNodeStruct.ContentClass));
						if (num < folderInfo.NumItems - 1U)
						{
							folderNodeStruct = (FolderNodeStruct)Marshal.PtrToStructure(folderNodeStruct.Next, typeof(FolderNodeStruct));
						}
					}
				}
				finally
				{
					SyncStateUpgrader.Foldersync_cleanup(folderInfo.Foldernodes);
				}
			}
			uint version;
			switch (folderInfo.Version)
			{
			case 65542U:
				version = 20U;
				break;
			case 65543U:
			case 65544U:
				version = 25U;
				break;
			default:
				version = 10U;
				break;
			}
			return itemSyncUpgrade.Upgrade(dictionary, new string(folderInfo.SyncKey), deviceIdentity, version, out contentTypeTable);
		}

		private const string ZeroString = "0000000000000000000000000000000000000000";
	}
}
