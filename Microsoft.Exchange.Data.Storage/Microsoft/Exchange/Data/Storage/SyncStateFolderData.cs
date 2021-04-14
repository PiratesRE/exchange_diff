using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Storage
{
	public class SyncStateFolderData
	{
		public string Name { get; set; }

		public DateTime Created { get; set; }

		public string StorageType { get; set; }

		public string SyncStateBlob { get; set; }

		public int SyncStateSize { get; set; }

		public List<FolderMappingData> FolderMapping { get; set; }
	}
}
