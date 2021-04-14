using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	[SimpleConfiguration("TargetFolderMRU", "TargetFolderMRU")]
	internal class TargetFolderMRUEntry
	{
		public TargetFolderMRUEntry()
		{
		}

		public TargetFolderMRUEntry(OwaStoreObjectId folderId)
		{
			this.folderId = folderId.ToString();
		}

		[SimpleConfigurationProperty("folderId")]
		public string FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		private string folderId;
	}
}
