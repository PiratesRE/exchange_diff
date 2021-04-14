using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class RetentionTag
	{
		internal RetentionTag()
		{
		}

		internal RetentionTag(RetentionPolicyTag retentionPolicyTag)
		{
			this.guid = retentionPolicyTag.Guid;
			this.effectiveGuid = retentionPolicyTag.RetentionId;
			this.name = retentionPolicyTag.Name;
			this.localizedRetentionPolicyTagName = retentionPolicyTag.LocalizedRetentionPolicyTagName.ToArray();
			this.comment = retentionPolicyTag.Comment;
			this.localizedComment = retentionPolicyTag.LocalizedComment.ToArray();
			this.type = retentionPolicyTag.Type;
			this.isPrimary = retentionPolicyTag.IsPrimary;
			this.mustDisplayCommentEnabled = retentionPolicyTag.MustDisplayCommentEnabled;
			if (retentionPolicyTag.LegacyManagedFolder != null)
			{
				this.legacyManagedFolder = new Guid?(retentionPolicyTag.LegacyManagedFolder.ObjectGuid);
			}
		}

		internal Guid Guid
		{
			get
			{
				if (this.guid == Guid.Empty)
				{
					this.guid = this.effectiveGuid;
				}
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		internal Guid RetentionId
		{
			get
			{
				return this.effectiveGuid;
			}
			set
			{
				this.effectiveGuid = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal string[] LocalizedRetentionPolicyTagName
		{
			get
			{
				return this.localizedRetentionPolicyTagName;
			}
			set
			{
				this.localizedRetentionPolicyTagName = value;
			}
		}

		internal string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		internal string[] LocalizedComment
		{
			get
			{
				return this.localizedComment;
			}
			set
			{
				this.localizedComment = value;
			}
		}

		internal ElcFolderType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal bool IsPrimary
		{
			get
			{
				return this.isPrimary;
			}
			set
			{
				this.isPrimary = value;
			}
		}

		internal bool MustDisplayCommentEnabled
		{
			get
			{
				return this.mustDisplayCommentEnabled;
			}
			set
			{
				this.mustDisplayCommentEnabled = value;
			}
		}

		internal Guid? LegacyManagedFolder
		{
			get
			{
				return this.legacyManagedFolder;
			}
			set
			{
				this.legacyManagedFolder = value;
			}
		}

		internal bool IsArchiveTag
		{
			get
			{
				return this.isArchiveTag;
			}
			set
			{
				this.isArchiveTag = value;
			}
		}

		private Guid guid;

		private string name;

		private string[] localizedRetentionPolicyTagName;

		private string comment;

		private string[] localizedComment;

		private ElcFolderType type;

		private bool isPrimary;

		private bool mustDisplayCommentEnabled;

		private Guid? legacyManagedFolder;

		private Guid effectiveGuid;

		private bool isArchiveTag;
	}
}
