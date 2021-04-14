using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	public struct FolderPathIndices
	{
		internal FolderPathIndices(int displayNameIndex, int folderDepthIndex, int folderIdIndex, int parentIdIndex, int folderPathIndex)
		{
			this.displayNameIndex = displayNameIndex;
			this.folderDepthIndex = folderDepthIndex;
			this.folderIdIndex = folderIdIndex;
			this.parentIdIndex = parentIdIndex;
			this.folderPathIndex = folderPathIndex;
		}

		internal int DisplayNameIndex
		{
			get
			{
				return this.displayNameIndex;
			}
		}

		internal int FolderDepthIndex
		{
			get
			{
				return this.folderDepthIndex;
			}
		}

		internal int FolderIdIndex
		{
			get
			{
				return this.folderIdIndex;
			}
		}

		internal int ParentIdIndex
		{
			get
			{
				return this.parentIdIndex;
			}
		}

		internal int FolderPathIndex
		{
			get
			{
				return this.folderPathIndex;
			}
		}

		private int displayNameIndex;

		private int folderDepthIndex;

		private int folderIdIndex;

		private int parentIdIndex;

		private int folderPathIndex;
	}
}
