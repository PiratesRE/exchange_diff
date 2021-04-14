using System;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class RestrictionRow : IConfigurable
	{
		ObjectId IConfigurable.Identity
		{
			get
			{
				return null;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return new ValidationError[0];
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.New;
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			if (!(source is RestrictionRow))
			{
				throw new NotImplementedException(string.Format("Cannot copy changes from type {0}.", source.GetType()));
			}
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		public RestrictionRow()
		{
			this.scopeFolderEntryId = null;
			this.displayName = null;
			this.cultureInfo = null;
			this.contentCount = 0L;
			this.contentUnread = 0L;
			this.viewAccessTime = null;
			this.restriction = null;
		}

		public RestrictionRow(MapiEntryId scopeFolderEntryId)
		{
			this.scopeFolderEntryId = scopeFolderEntryId;
			this.displayName = null;
			this.cultureInfo = null;
			this.contentCount = 0L;
			this.contentUnread = 0L;
			this.viewAccessTime = null;
			this.restriction = null;
		}

		public MapiEntryId ScopeFolderEntryId
		{
			get
			{
				return this.scopeFolderEntryId;
			}
			internal set
			{
				this.scopeFolderEntryId = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			internal set
			{
				this.displayName = value;
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				return this.cultureInfo;
			}
			internal set
			{
				this.cultureInfo = value;
			}
		}

		public long ContentCount
		{
			get
			{
				return this.contentCount;
			}
			internal set
			{
				this.contentCount = value;
			}
		}

		public long ContentUnread
		{
			get
			{
				return this.contentUnread;
			}
			internal set
			{
				this.contentUnread = value;
			}
		}

		public DateTime? ViewAccessTime
		{
			get
			{
				return this.viewAccessTime;
			}
			internal set
			{
				this.viewAccessTime = value;
			}
		}

		public MapiEntryId MapiEntryID
		{
			get
			{
				return this.restrictionEntryId;
			}
			internal set
			{
				this.restrictionEntryId = value;
			}
		}

		public string Restriction
		{
			get
			{
				string text = this.restriction;
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					text = SuppressingPiiData.Redact(text);
				}
				return text;
			}
			internal set
			{
				this.restriction = value;
			}
		}

		private MapiEntryId scopeFolderEntryId;

		private string displayName;

		private CultureInfo cultureInfo;

		private long contentCount;

		private long contentUnread;

		private DateTime? viewAccessTime;

		private MapiEntryId restrictionEntryId;

		private string restriction;
	}
}
