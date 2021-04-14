using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	[Serializable]
	internal sealed class AgentSubscription : IDisposable
	{
		public AgentSubscription(string name, string[] agents, string[][] eventTopics)
		{
			this.name = name;
			this.agents = agents;
			this.Update(eventTopics);
		}

		public IEnumerable<string> EventTopics
		{
			get
			{
				return this.topics;
			}
		}

		internal long Size
		{
			get
			{
				long length;
				using (MemoryStream memoryStream = new MemoryStream(1024))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memoryStream, this);
					length = memoryStream.Length;
				}
				return length;
			}
		}

		public IEnumerable<string> this[string eventTopic]
		{
			get
			{
				this.EnsureInitialized();
				if (!this.subscriptions.ContainsKey(eventTopic))
				{
					return AgentSubscription.EmptyList;
				}
				return this.subscriptions[eventTopic];
			}
		}

		public static AgentSubscription Load(string name)
		{
			MemoryMappedFile readOnlyFile = AgentSubscription.GetReadOnlyFile(name);
			if (readOnlyFile == null)
			{
				return null;
			}
			AgentSubscription result;
			try
			{
				int num = 24;
				int num2 = 50;
				do
				{
					Thread.Sleep(10);
					result = null;
					using (MapFileStream mapFileStream = readOnlyFile.CreateView(0, num))
					{
						byte[] array = new byte[24L];
						int num3 = mapFileStream.Read(array, 0, num);
						if (num3 == num)
						{
							AgentSubscription.Header header = new AgentSubscription.Header(array);
							if (header.Magic == 96101745125713L && header.PayloadSize > 0L && header.PayloadSize <= 1048576L)
							{
								using (MapFileStream mapFileStream2 = readOnlyFile.CreateView(0, num + (int)header.PayloadSize))
								{
									mapFileStream2.Seek((long)num, SeekOrigin.Begin);
									BinaryFormatter binaryFormatter = new BinaryFormatter();
									try
									{
										result = (AgentSubscription)binaryFormatter.Deserialize(mapFileStream2);
									}
									catch (SerializationException)
									{
										goto IL_137;
									}
								}
								mapFileStream.Seek(0L, SeekOrigin.Begin);
								num3 = mapFileStream.Read(array, 0, num);
								if (num3 == num)
								{
									AgentSubscription.Header header2 = new AgentSubscription.Header(array);
									if (header.Magic == header2.Magic && header.PayloadSize == header2.PayloadSize && header.Ticks == header2.Ticks)
									{
										break;
									}
								}
							}
						}
					}
					IL_137:;
				}
				while (num2-- > 0);
			}
			finally
			{
				readOnlyFile.Close();
			}
			return result;
		}

		public void Dispose()
		{
			if (this.mappedFile != null)
			{
				this.mappedFile.Close();
			}
		}

		internal void Save()
		{
			if (this.topics.Length > 1000 || this.agents.Length > 1000)
			{
				return;
			}
			long size = this.Size;
			if (size > 1048576L)
			{
				return;
			}
			MemoryMappedFile writableFile = this.GetWritableFile();
			if (writableFile == null)
			{
				return;
			}
			using (MapFileStream mapFileStream = writableFile.CreateView(0, (int)(size + 24L)))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				byte[] array = new byte[24L];
				mapFileStream.Write(array, 0, array.Length);
				mapFileStream.Flush();
				binaryFormatter.Serialize(mapFileStream, this);
				AgentSubscription.Header header = new AgentSubscription.Header(size);
				array = header.GetBytes();
				mapFileStream.Seek(0L, SeekOrigin.Begin);
				mapFileStream.Write(array, 0, array.Length);
				mapFileStream.Flush();
			}
		}

		internal void Update(string[][] eventTopics)
		{
			this.subscriptions = null;
			Dictionary<string, short> dictionary = new Dictionary<string, short>();
			short num = 0;
			for (int i = 0; i < eventTopics.Length; i++)
			{
				for (int j = 0; j < eventTopics[i].Length; j++)
				{
					if (!dictionary.ContainsKey(eventTopics[i][j]))
					{
						Dictionary<string, short> dictionary2 = dictionary;
						string key = eventTopics[i][j];
						short num2 = num;
						num = num2 + 1;
						dictionary2.Add(key, num2);
					}
				}
			}
			this.topics = new string[dictionary.Count];
			dictionary.Keys.CopyTo(this.topics, 0);
			this.subscriptionTable = new short[this.agents.Length][];
			for (int k = 0; k < this.subscriptionTable.Length; k++)
			{
				this.subscriptionTable[k] = new short[eventTopics[k].Length];
				for (int l = 0; l < eventTopics[k].Length; l++)
				{
					this.subscriptionTable[k][l] = dictionary[eventTopics[k][l]];
				}
			}
		}

		private static MemoryMappedFile GetReadOnlyFile(string name)
		{
			return AgentSubscription.GetMemoryMappedFile(name, false);
		}

		private static MemoryMappedFile GetMemoryMappedFile(string name, bool writable)
		{
			MemoryMappedFile result = null;
			for (int i = 0; i < 10; i++)
			{
				try
				{
					result = new MemoryMappedFile("Global\\MExRuntimeAgentSubscription" + AgentSubscription.Digits[i] + name, 1048600, writable);
					break;
				}
				catch (IOException)
				{
				}
			}
			return result;
		}

		private void EnsureInitialized()
		{
			if (this.subscriptions != null)
			{
				return;
			}
			this.subscriptions = new Dictionary<string, List<string>>();
			for (int i = 0; i < this.agents.Length; i++)
			{
				for (int j = 0; j < this.subscriptionTable[i].Length; j++)
				{
					string key = this.topics[(int)this.subscriptionTable[i][j]];
					if (!this.subscriptions.ContainsKey(key))
					{
						this.subscriptions.Add(key, new List<string>());
					}
					this.subscriptions[key].Add(this.agents[i]);
				}
			}
		}

		private MemoryMappedFile GetWritableFile()
		{
			if (this.mappedFile != null)
			{
				return this.mappedFile;
			}
			this.mappedFile = AgentSubscription.GetMemoryMappedFile(this.name, true);
			return this.mappedFile;
		}

		private const int MaxSize = 1048576;

		private const int MaxEntries = 1000;

		private const string FileNamePrefix = "Global\\MExRuntimeAgentSubscription";

		[NonSerialized]
		private static readonly string[] Digits = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9"
		};

		[NonSerialized]
		private static readonly List<string> EmptyList = new List<string>(0);

		private string name;

		private string[] agents;

		private string[] topics;

		private short[][] subscriptionTable;

		[NonSerialized]
		private Dictionary<string, List<string>> subscriptions;

		[NonSerialized]
		private MemoryMappedFile mappedFile;

		private struct Header
		{
			public Header(long payloadSize)
			{
				this.magic = 96101745125713L;
				this.payloadSize = payloadSize;
				this.ticks = DateTime.UtcNow.Ticks;
			}

			public Header(byte[] data)
			{
				this.magic = BitConverter.ToInt64(data, 0);
				this.payloadSize = BitConverter.ToInt64(data, 8);
				this.ticks = BitConverter.ToInt64(data, 16);
			}

			public long Magic
			{
				get
				{
					return this.magic;
				}
			}

			public long PayloadSize
			{
				get
				{
					return this.payloadSize;
				}
			}

			public long Ticks
			{
				get
				{
					return this.ticks;
				}
			}

			public byte[] GetBytes()
			{
				byte[] bytes = BitConverter.GetBytes(this.magic);
				byte[] bytes2 = BitConverter.GetBytes(this.payloadSize);
				byte[] bytes3 = BitConverter.GetBytes(this.ticks);
				byte[] array = new byte[24L];
				bytes.CopyTo(array, 0);
				bytes2.CopyTo(array, 8);
				bytes3.CopyTo(array, 16);
				return array;
			}

			public const long AgentSubscriptionMagic = 96101745125713L;

			public const long Size = 24L;

			private long magic;

			private long payloadSize;

			private long ticks;
		}
	}
}
