using System;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Data.Directory.UnifiedPolicy
{
	[Serializable]
	internal sealed class PolicySyncCookie : ISerializable
	{
		public PolicySyncCookie()
		{
		}

		public PolicySyncCookie(SerializationInfo info, StreamingContext context)
		{
			this.keyValueStorage = (PolicyKeyStorage)info.GetValue("KeyValueStorage", typeof(PolicyKeyStorage));
		}

		public string this[string key]
		{
			get
			{
				return this.keyValueStorage[key];
			}
			set
			{
				this.keyValueStorage[key] = value;
			}
		}

		public static PolicySyncCookie Deserialize(byte[] bytes)
		{
			if (bytes == null)
			{
				return new PolicySyncCookie();
			}
			PolicySyncCookie result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				PolicySyncCookie policySyncCookie = PolicySyncCookie.GetSerializer().ReadObject(memoryStream) as PolicySyncCookie;
				if (policySyncCookie == null)
				{
					throw new InvalidOperationException("Failed to deserialize cookie data");
				}
				result = policySyncCookie;
			}
			return result;
		}

		public byte[] Serialize()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PolicySyncCookie.GetSerializer().WriteObject(memoryStream, this);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public bool TryGetValue(string key, out string result)
		{
			return this.keyValueStorage.TryGetValue(key, out result);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("KeyValueStorage", this.keyValueStorage);
		}

		private static DataContractSerializer GetSerializer()
		{
			return new DataContractSerializer(typeof(PolicySyncCookie), new Type[]
			{
				typeof(PolicyKeyStorage)
			});
		}

		private PolicyKeyStorage keyValueStorage = new PolicyKeyStorage();
	}
}
