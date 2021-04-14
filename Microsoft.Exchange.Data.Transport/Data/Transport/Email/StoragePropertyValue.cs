using System;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class StoragePropertyValue
	{
		public StoragePropertyValue(TnefPropertyTag propertyTag, DataStorage storage, long start, long end)
		{
			if (storage != null)
			{
				storage.AddRef();
			}
			this.propertyTag = propertyTag;
			this.storage = storage;
			this.start = start;
			this.end = end;
		}

		public TnefPropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public DataStorage Storage
		{
			get
			{
				return this.storage;
			}
		}

		public long Start
		{
			get
			{
				return this.start;
			}
		}

		public long End
		{
			get
			{
				return this.end;
			}
		}

		public Stream GetReadStream()
		{
			return this.storage.OpenReadStream(this.start, this.end);
		}

		public void SetUnicodePropertyTag()
		{
			this.propertyTag = new TnefPropertyTag(this.propertyTag.Id, TnefPropertyType.Unicode);
		}

		public void SetBinaryPropertyTag()
		{
			this.propertyTag = new TnefPropertyTag(this.propertyTag.Id, TnefPropertyType.Binary);
		}

		public void SetStorage(DataStorage storage, long start, long end)
		{
			if (this.storage != null)
			{
				this.storage.Release();
			}
			if (storage != null)
			{
				storage.AddRef();
			}
			this.storage = storage;
			this.start = start;
			this.end = end;
		}

		public void DisposeStorage()
		{
			if (this.storage != null)
			{
				this.storage.Release();
				this.storage = null;
			}
		}

		private TnefPropertyTag propertyTag;

		private DataStorage storage;

		private long start;

		private long end;
	}
}
