using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TopicHashCache
	{
		internal static TopicHashCache Load(IXSOFactory xsoFactory, IMailboxSession session)
		{
			return TopicHashCache.Load(xsoFactory, session, 50);
		}

		internal static TopicHashCache Load(IXSOFactory xsoFactory, IMailboxSession session, int cacheSize)
		{
			TopicHashCache topicHashCache = new TopicHashCache(cacheSize);
			try
			{
				byte[] array = null;
				using (IFolder folder = xsoFactory.BindToFolder(session, DefaultFolderType.Inbox, new PropertyDefinition[]
				{
					FolderSchema.ConversationTopicHashEntries
				}))
				{
					array = (folder.TryGetProperty(FolderSchema.ConversationTopicHashEntries) as byte[]);
				}
				if (array != null)
				{
					using (MemoryStream memoryStream = new MemoryStream(array))
					{
						topicHashCache.Deserialize(memoryStream);
					}
				}
			}
			catch (IOException arg)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<IOException>(0L, "TopicHashCache::Load. Encountered the following exception. Exception = {0}.", arg);
				topicHashCache = new TopicHashCache(cacheSize);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StorageTransientException>(0L, "TopicHashCache::Load. Encountered the following exception. Exception = {0}.", arg2);
				topicHashCache = new TopicHashCache(cacheSize);
			}
			catch (StoragePermanentException arg3)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StoragePermanentException>(0L, "TopicHashCache::Load. Encountered the following exception. Exception = {0}.", arg3);
				topicHashCache = new TopicHashCache(cacheSize);
			}
			return topicHashCache;
		}

		internal static void Save(TopicHashCache cache, IXSOFactory xsoFactory, IMailboxSession session)
		{
			try
			{
				using (IFolder folder = xsoFactory.BindToFolder(session, DefaultFolderType.Inbox, new PropertyDefinition[]
				{
					FolderSchema.ConversationTopicHashEntries
				}))
				{
					using (MemoryStream memoryStream = new MemoryStream(cache.EstimatedSize))
					{
						cache.Serialize(memoryStream);
						folder[FolderSchema.ConversationTopicHashEntries] = memoryStream.ToArray();
						folder.Save();
					}
				}
			}
			catch (IOException arg)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<IOException>(0L, "TopicHashCache::Save. Encountered the following exception. Exception = {0}.", arg);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StorageTransientException>(0L, "TopicHashCache::Load. Encountered the following exception. Exception = {0}.", arg2);
			}
			catch (StoragePermanentException arg3)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StoragePermanentException>(0L, "TopicHashCache::Load. Encountered the following exception. Exception = {0}.", arg3);
			}
		}

		private TopicHashCache(int cacheSize)
		{
			this.TopicHashCacheSize = cacheSize;
			this.topicHashes = new List<uint>(this.TopicHashCacheSize);
		}

		internal ExDateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
		}

		private int Index
		{
			get
			{
				return this.index;
			}
		}

		internal int Count
		{
			get
			{
				return this.topicHashes.Count;
			}
		}

		internal int Capacity
		{
			get
			{
				return this.TopicHashCacheSize;
			}
		}

		internal ReadOnlyCollection<uint> TopicHashes
		{
			get
			{
				return new ReadOnlyCollection<uint>(this.topicHashes);
			}
		}

		private int EstimatedSize
		{
			get
			{
				return 4 * this.topicHashes.Count + 20;
			}
		}

		internal void Add(uint hash)
		{
			int num = (this.index == this.TopicHashCacheSize - 1) ? 0 : (this.index + 1);
			if (this.topicHashes.Count == this.TopicHashCacheSize)
			{
				this.topicHashes[num] = hash;
			}
			else
			{
				this.topicHashes.Add(hash);
			}
			this.index = num;
			this.lastModifiedTime = ExDateTime.Now.ToUtc();
		}

		internal bool Contains(uint hash)
		{
			return this.index != -1 && this.topicHashes.Contains(hash);
		}

		private void Serialize(MemoryStream stream)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.Write(this.lastModifiedTime.ToBinary());
				binaryWriter.Write(this.TopicHashCacheSize);
				binaryWriter.Write(this.index);
				binaryWriter.Write(this.topicHashes.Count);
				for (int i = 0; i < this.topicHashes.Count; i++)
				{
					binaryWriter.Write(this.topicHashes[i]);
				}
			}
		}

		private void Deserialize(MemoryStream stream)
		{
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				if (binaryReader.BaseStream.Length != 0L)
				{
					ExDateTime dt = ExDateTime.FromBinary(binaryReader.ReadInt64());
					if ((ExDateTime.Now.ToUtc() - dt).TotalDays <= 3.0)
					{
						int num = binaryReader.ReadInt32();
						if (this.TopicHashCacheSize == num)
						{
							this.lastModifiedTime = dt;
							this.index = binaryReader.ReadInt32();
							int num2 = binaryReader.ReadInt32();
							for (int i = 0; i < num2; i++)
							{
								this.topicHashes.Add(binaryReader.ReadUInt32());
							}
						}
					}
				}
			}
		}

		private const int DefaultCacheSize = 50;

		private const int DiscardCacheAfterDays = 3;

		private readonly int TopicHashCacheSize = 50;

		private ExDateTime lastModifiedTime = ExDateTime.MinValue;

		private int index = -1;

		private List<uint> topicHashes;
	}
}
