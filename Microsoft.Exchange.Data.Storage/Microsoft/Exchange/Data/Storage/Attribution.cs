using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class Attribution
	{
		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public AttributionSourceId SourceId
		{
			get
			{
				return this.sourceId;
			}
			set
			{
				this.sourceId = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public bool IsWritable
		{
			get
			{
				return this.isWritable;
			}
			set
			{
				this.isWritable = value;
			}
		}

		public bool IsQuickContact
		{
			get
			{
				return this.isQuickContact;
			}
			set
			{
				this.isQuickContact = value;
			}
		}

		public StoreObjectId FolderId { get; set; }

		public bool IsHidden { get; set; }

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
			set
			{
				this.folderName = value;
			}
		}

		private string id;

		private AttributionSourceId sourceId;

		private string displayName;

		private bool isWritable;

		private bool isQuickContact;

		private string folderName;
	}
}
