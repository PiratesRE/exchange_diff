using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Net.Mserve
{
	[Serializable]
	internal sealed class RecipientSyncState
	{
		public static RecipientSyncState DeserializeRecipientSyncState(byte[] data)
		{
			RecipientSyncState result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				RecipientSyncState recipientSyncState = (RecipientSyncState)RecipientSyncState.serializer.Deserialize(memoryStream);
				result = recipientSyncState;
			}
			return result;
		}

		public static byte[] SerializeRecipientSyncState(RecipientSyncState state)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				RecipientSyncState.serializer.Serialize(memoryStream, state);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static HashSet<string> AddressHashSetFromConcatStringValue(string addresses)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (!string.IsNullOrEmpty(addresses))
			{
				string[] separator = new string[]
				{
					";"
				};
				string[] array = addresses.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string item in array)
				{
					if (!hashSet.Contains(item))
					{
						hashSet.Add(item);
					}
				}
			}
			return hashSet;
		}

		public static string AddressHashSetToConcatStringValue(HashSet<string> set)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in set)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		public static List<string> AddressToList(string addresses)
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(addresses))
			{
				string[] array = addresses.Split(new char[]
				{
					';'
				});
				foreach (string text in array)
				{
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		public string ProxyAddresses;

		public string SignupAddresses;

		public int PartnerId;

		public string UMProxyAddresses;

		public string ArchiveAddress;

		private static readonly RecipientSyncStateSerializer serializer = new RecipientSyncStateSerializer();
	}
}
