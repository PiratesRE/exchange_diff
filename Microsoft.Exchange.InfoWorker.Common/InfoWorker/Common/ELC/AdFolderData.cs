using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class AdFolderData
	{
		public ELCFolder Folder
		{
			get
			{
				return this.folder;
			}
			set
			{
				this.folder = value;
			}
		}

		public ContentSetting[] FolderSettings
		{
			get
			{
				return this.folderSettings;
			}
			set
			{
				this.folderSettings = value;
			}
		}

		public bool LinkedToTemplate
		{
			get
			{
				return this.linkedToTemplate;
			}
			set
			{
				this.linkedToTemplate = value;
			}
		}

		public bool Synced
		{
			get
			{
				return this.synced;
			}
			set
			{
				this.synced = value;
			}
		}

		private ELCFolder folder;

		private ContentSetting[] folderSettings;

		private bool linkedToTemplate;

		private bool synced;
	}
}
