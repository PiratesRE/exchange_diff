using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public sealed class FolderEntry
	{
		public FolderEntry(ExchangeId folderId, short specialFolderNumber, string displayName)
		{
			this.folderId = folderId;
			this.specialFolderNumber = specialFolderNumber;
			this.displayName = displayName;
		}

		public ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public short SpecialFolderNumber
		{
			get
			{
				return this.specialFolderNumber;
			}
		}

		public bool NameStartsWith(string prefix)
		{
			return !string.IsNullOrEmpty(this.displayName) && this.displayName.StartsWith(prefix);
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", this.FolderId, this.SpecialFolderNumber);
		}

		private readonly ExchangeId folderId;

		private readonly short specialFolderNumber;

		private readonly string displayName;
	}
}
