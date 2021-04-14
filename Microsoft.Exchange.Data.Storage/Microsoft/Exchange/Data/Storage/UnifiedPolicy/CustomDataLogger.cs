using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CustomDataLogger
	{
		static CustomDataLogger()
		{
			int num = 0;
			foreach (string key in CustomDataLogger.CustomFields)
			{
				CustomDataLogger.customFieldMappingTable[key] = CustomDataLogger.startingIndex + num;
				num++;
			}
		}

		public static string GetFieldFromTag(CustomDataLogger.Tag tag)
		{
			return CustomDataLogger.CustomFields[tag - (CustomDataLogger.Tag)CustomDataLogger.startingIndex];
		}

		public static void Log(KeyValuePair<string, object>[] customData, LogRowFormatter logRow, out Stream storeStream)
		{
			storeStream = null;
			if (customData != null)
			{
				if (logRow == null)
				{
					throw new ArgumentNullException("logRow");
				}
				foreach (KeyValuePair<string, object> keyValuePair in customData)
				{
					if (!CustomDataLogger.customFieldMappingTable.ContainsKey(keyValuePair.Key))
					{
						throw new ArgumentException(string.Format("Invalid custom field name {0}", keyValuePair.Key));
					}
					if (CustomDataLogger.customFieldMappingTable[keyValuePair.Key] != 16)
					{
						logRow[CustomDataLogger.customFieldMappingTable[keyValuePair.Key]] = keyValuePair.Value;
					}
					else
					{
						storeStream = (Stream)keyValuePair.Value;
					}
				}
			}
		}

		public static readonly string[] CustomFields = new string[]
		{
			"Subcomponent",
			"SyncMailboxGuid",
			"SyncSvcUrl",
			"StoreStream"
		};

		private static Dictionary<string, int> customFieldMappingTable = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		private static int startingIndex = 13;

		public enum Tag
		{
			Subcomponent = 13,
			SyncMailboxGuid,
			SyncSvcUrl,
			StoreStream
		}
	}
}
