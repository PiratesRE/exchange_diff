using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Exchange.Common.Cache
{
	internal class BufferCache
	{
		public BufferCache(int cacheSize = 1000)
		{
			this.maxCacheSize = cacheSize;
		}

		public BufferCacheEntry GetBuffer(int size)
		{
			lock (this)
			{
				Queue<LinkedListNode<BufferCacheEntry>> queue;
				if (BufferCache.IsSupportedBufferSize(size) && this.sizeToBufferMap.TryGetValue(size, out queue))
				{
					this.hitCount++;
					LinkedListNode<BufferCacheEntry> linkedListNode = this.ReleaseBuffer(queue);
					this.bufferQueue.Remove(linkedListNode);
					if (linkedListNode.Value.Buffer.Length != size)
					{
						throw new InvalidOperationException(string.Format("Unexpected count {0} for buffer. Expected count was {1}", linkedListNode.Value.Buffer.Length, size));
					}
					Array.Clear(linkedListNode.Value.Buffer, 0, linkedListNode.Value.Buffer.Length);
					return linkedListNode.Value;
				}
				else
				{
					this.missCount++;
				}
			}
			return new BufferCacheEntry(new byte[size], true);
		}

		public void ReturnBuffer(BufferCacheEntry bufferCacheEntry)
		{
			if (bufferCacheEntry.OwnedByBufferCache && BufferCache.IsSupportedBufferSize(bufferCacheEntry.Buffer.Length))
			{
				lock (this)
				{
					this.returnedBufferCount++;
					if (this.bufferQueue.Count == this.maxCacheSize)
					{
						LinkedListNode<BufferCacheEntry> first = this.bufferQueue.First;
						int key = first.Value.Buffer.Length;
						if (!object.ReferenceEquals(this.sizeToBufferMap[key].Peek(), first))
						{
							throw new InvalidOperationException("Inconsistent datastructure detected in BufferCache");
						}
						this.bufferQueue.RemoveFirst();
						this.ReleaseBuffer(this.sizeToBufferMap[key]);
					}
					LinkedListNode<BufferCacheEntry> bufferNode = this.bufferQueue.AddLast(bufferCacheEntry);
					Queue<LinkedListNode<BufferCacheEntry>> queue;
					if (!this.sizeToBufferMap.TryGetValue(bufferCacheEntry.Buffer.Length, out queue))
					{
						queue = new Queue<LinkedListNode<BufferCacheEntry>>();
						this.sizeToBufferMap[bufferCacheEntry.Buffer.Length] = queue;
					}
					this.AddBuffer(bufferNode, queue);
				}
			}
		}

		private static bool IsSupportedBufferSize(int size)
		{
			return size % BufferCache.OneKiloByteBufferSize == 0 && size <= BufferCache.MaxSupportedByteArraySize;
		}

		private void AddBuffer(LinkedListNode<BufferCacheEntry> bufferNode, Queue<LinkedListNode<BufferCacheEntry>> queue)
		{
			if (!this.freeBufferSet.Add(bufferNode.Value))
			{
				throw new InvalidOperationException("Trying to add the same buffer twice");
			}
			queue.Enqueue(bufferNode);
		}

		private LinkedListNode<BufferCacheEntry> ReleaseBuffer(Queue<LinkedListNode<BufferCacheEntry>> queue)
		{
			LinkedListNode<BufferCacheEntry> linkedListNode = queue.Dequeue();
			if (queue.Count == 0)
			{
				this.sizeToBufferMap.Remove(linkedListNode.Value.Buffer.Length);
			}
			if (!this.freeBufferSet.Remove(linkedListNode.Value))
			{
				throw new InvalidOperationException(string.Format("Did not find entry {0}", linkedListNode.Value.Buffer.Length));
			}
			return linkedListNode;
		}

		public void AddDiagnosticInfoTo(XElement cacheElement, bool verbose)
		{
			cacheElement.SetAttributeValue("HitCount", this.hitCount);
			cacheElement.SetAttributeValue("MissCount", this.missCount);
			if (!verbose)
			{
				return;
			}
			foreach (KeyValuePair<int, Queue<LinkedListNode<BufferCacheEntry>>> keyValuePair in this.sizeToBufferMap)
			{
				XElement xelement = new XElement("entry");
				xelement.SetAttributeValue("Size", keyValuePair.Key);
				xelement.SetAttributeValue("Count", keyValuePair.Value.Count);
				cacheElement.Add(xelement);
			}
		}

		public static readonly int OneKiloByteBufferSize = 1024;

		private static readonly int MaxSupportedByteArraySize = BufferCache.OneKiloByteBufferSize * 100;

		private readonly int maxCacheSize;

		private Dictionary<int, Queue<LinkedListNode<BufferCacheEntry>>> sizeToBufferMap = new Dictionary<int, Queue<LinkedListNode<BufferCacheEntry>>>();

		private LinkedList<BufferCacheEntry> bufferQueue = new LinkedList<BufferCacheEntry>();

		private HashSet<BufferCacheEntry> freeBufferSet = new HashSet<BufferCacheEntry>();

		private int hitCount;

		private int missCount;

		private int returnedBufferCount;
	}
}
