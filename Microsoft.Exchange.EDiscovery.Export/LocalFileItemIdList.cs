using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class LocalFileItemIdList : IItemIdList
	{
		public LocalFileItemIdList(string sourceId, string filePath, bool isUnsearchable)
		{
			this.SourceId = sourceId;
			this.memoryCache = new List<ItemId>(ConstantProvider.ItemIdListCacheSize);
			this.filePath = filePath;
			this.IsUnsearchable = isUnsearchable;
		}

		public string SourceId { get; private set; }

		public IList<ItemId> MemoryCache
		{
			get
			{
				return this.memoryCache;
			}
		}

		public bool Exists
		{
			get
			{
				if (this.exists == null)
				{
					this.exists = new bool?(File.Exists(this.filePath));
				}
				return this.exists.Value;
			}
		}

		public bool IsUnsearchable { get; private set; }

		public void WriteItemId(ItemId itemId)
		{
			this.memoryCache.Add(itemId);
		}

		public void Flush()
		{
			LocalFileHelper.CallFileOperation(delegate
			{
				if (this.memoryCache.Count > 0)
				{
					using (FileStream fileStream = new FileStream(this.filePath, FileMode.Append, FileAccess.Write))
					{
						foreach (ItemId itemId in this.memoryCache)
						{
							itemId.WriteToStream(fileStream);
							if (this.IsUnsearchable && !(itemId is UnsearchableItemId))
							{
								UnsearchableItemId.WriteDummyToStream(fileStream);
							}
						}
					}
					Tracer.TraceInformation("LocalFileItemList.Flush: {0} item IDs are written into '{1}'", new object[]
					{
						this.memoryCache.Count,
						this.filePath
					});
				}
				this.memoryCache.Clear();
			}, ExportErrorType.FailedToWriteItemIdList);
		}

		public IEnumerable<ItemId> ReadItemIds()
		{
			if (this.Exists)
			{
				FileStream fileStream = null;
				try
				{
					LocalFileHelper.CallFileOperation(delegate
					{
						fileStream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read);
					}, ExportErrorType.FailedToReadItemIdList);
					while (fileStream.Position < fileStream.Length)
					{
						ItemId itemId = this.IsUnsearchable ? new UnsearchableItemId() : new ItemId();
						LocalFileHelper.CallFileOperation(delegate
						{
							itemId.ReadFromStream(fileStream, this.SourceId);
						}, ExportErrorType.FailedToReadItemIdList);
						yield return itemId;
					}
				}
				finally
				{
					if (fileStream != null)
					{
						fileStream.Flush();
						fileStream.Dispose();
					}
				}
			}
			yield break;
		}

		private readonly string filePath;

		private List<ItemId> memoryCache;

		private bool? exists = null;
	}
}
