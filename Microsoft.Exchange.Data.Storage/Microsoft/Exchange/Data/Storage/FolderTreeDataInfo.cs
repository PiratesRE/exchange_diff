using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FolderTreeDataInfo
	{
		public VersionedId Id { get; private set; }

		public byte[] Ordinal { get; private set; }

		public ExDateTime LastModifiedTime { get; private set; }

		public FolderTreeDataInfo(VersionedId id, byte[] ordinal, ExDateTime lastModifiedTime)
		{
			Util.ThrowOnNullArgument(id, "id");
			this.Id = id;
			this.Ordinal = ordinal;
			this.LastModifiedTime = lastModifiedTime;
		}
	}
}
