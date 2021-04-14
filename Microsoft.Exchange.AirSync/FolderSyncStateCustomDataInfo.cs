using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal struct FolderSyncStateCustomDataInfo
	{
		internal static void HandlerCustomDataVersioning(FolderSyncState syncState)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (syncState.CustomVersion == null && syncState.SyncStateIsNew)
			{
				return;
			}
			bool flag = true;
			if (syncState.CustomVersion == null || syncState.CustomVersion <= 2 || syncState.CustomVersion > 9)
			{
				flag = false;
			}
			else if (syncState.CustomVersion.Value != 9)
			{
				switch (syncState.CustomVersion.Value)
				{
				case 3:
					syncState["CachedOptionsNode"] = null;
					break;
				case 4:
					break;
				case 5:
					goto IL_117;
				case 6:
					goto IL_12C;
				case 7:
					goto IL_13D;
				case 8:
					goto IL_143;
				default:
					flag = false;
					goto IL_158;
				}
				object obj = syncState[CustomStateDatumType.AirSyncProtocolVersion];
				if (obj is ConstStringData)
				{
					string data = syncState.GetData<ConstStringData, string>(CustomStateDatumType.AirSyncProtocolVersion, null);
					int data2 = 20;
					if (data != null)
					{
						data2 = AirSyncUtility.ParseVersionString(data);
					}
					syncState[CustomStateDatumType.AirSyncProtocolVersion] = new Int32Data(data2);
				}
				IL_117:
				syncState["MaxItems"] = new Int32Data(int.MaxValue);
				IL_12C:
				syncState["ConversationMode"] = new BooleanData(false);
				IL_13D:
				FolderSyncStateCustomDataInfo.ConvertV7StickyOptions(syncState);
				IL_143:
				syncState["Permissions"] = new Int32Data(0);
			}
			IL_158:
			if (!flag)
			{
				syncState.HandleCorruptSyncState();
			}
		}

		private static void ConvertV7StickyOptions(FolderSyncState syncState)
		{
			XmlNode xmlNode = null;
			ByteArrayData byteArrayData = (ByteArrayData)syncState[CustomStateDatumType.CachedOptionsNode];
			if (byteArrayData == null || byteArrayData.Data == null)
			{
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(byteArrayData.Data))
			{
				using (WbxmlReader wbxmlReader = new WbxmlReader(memoryStream))
				{
					xmlNode = wbxmlReader.ReadXmlDocument().FirstChild;
					if (xmlNode == null)
					{
						return;
					}
				}
			}
			using (MemoryStream memoryStream2 = new MemoryStream(50))
			{
				WbxmlWriter wbxmlWriter = new WbxmlWriter(memoryStream2);
				XmlElement xmlElement = xmlNode.OwnerDocument.CreateElement("Collection", "AirSync:");
				xmlElement.AppendChild(xmlNode);
				wbxmlWriter.WriteXmlDocumentFromElement(xmlElement);
				syncState[CustomStateDatumType.CachedOptionsNode] = new ByteArrayData(memoryStream2.ToArray());
			}
		}

		internal const string AirSyncClassType = "AirSyncClassType";

		internal const string AirSyncProtocolVersion = "AirSyncProtocolVersion";

		internal const string CalendarSyncState = "CalendarSyncState";

		internal const string RecoveryCalendarSyncState = "RecoveryCalendarSyncState";

		internal const string CalendarMasterItems = "CalendarMasterItems";

		internal const string CustomCalendarSyncFilter = "CustomCalendarSyncFilter";

		internal const string FilterType = "FilterType";

		internal const string MaxItems = "MaxItems";

		internal const string ConversationMode = "ConversationMode";

		internal const string IdMapping = "IdMapping";

		internal const string RecoverySyncKey = "RecoverySyncKey";

		internal const string SupportedTags = "SupportedTags";

		internal const string SyncKey = "SyncKey";

		internal const string CachedOptionsNode = "CachedOptionsNode";

		internal const string Permissions = "Permissions";

		internal const int Version = 9;

		internal const int E12BaseVersion = 2;
	}
}
