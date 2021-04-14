using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class PSTSyncStateDictionary : XMLSerializableBase
	{
		public PSTSyncStateDictionary()
		{
			this.syncStates = new EntryIdMap<string>();
		}

		[XmlArrayItem("SyncState")]
		[XmlArray("SyncStates")]
		public PSTSyncStateDictionary.SyncStatePair[] SyncStates
		{
			get
			{
				PSTSyncStateDictionary.SyncStatePair[] array = new PSTSyncStateDictionary.SyncStatePair[this.syncStates.Count];
				int num = 0;
				foreach (KeyValuePair<byte[], string> keyValuePair in this.syncStates)
				{
					PSTSyncStateDictionary.SyncStatePair syncStatePair = new PSTSyncStateDictionary.SyncStatePair();
					syncStatePair.Key = keyValuePair.Key;
					syncStatePair.SyncState = keyValuePair.Value;
					array[num++] = syncStatePair;
				}
				return array;
			}
			set
			{
				this.syncStates.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						PSTSyncStateDictionary.SyncStatePair syncStatePair = value[i];
						this.syncStates[syncStatePair.Key] = syncStatePair.SyncState;
					}
				}
			}
		}

		[XmlIgnore]
		public string this[byte[] key]
		{
			get
			{
				string result;
				if (!this.syncStates.TryGetValue(key, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				this.syncStates[key] = value;
			}
		}

		internal static PSTSyncStateDictionary Deserialize(string blob)
		{
			return XMLSerializableBase.Deserialize<PSTSyncStateDictionary>(blob, false);
		}

		private EntryIdMap<string> syncStates;

		public class SyncStatePair
		{
			[XmlElement]
			public byte[] Key { get; set; }

			[XmlElement]
			public string SyncState { get; set; }
		}
	}
}
