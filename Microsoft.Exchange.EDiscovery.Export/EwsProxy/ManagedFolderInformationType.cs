using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class ManagedFolderInformationType
	{
		public bool CanDelete
		{
			get
			{
				return this.canDeleteField;
			}
			set
			{
				this.canDeleteField = value;
			}
		}

		[XmlIgnore]
		public bool CanDeleteSpecified
		{
			get
			{
				return this.canDeleteFieldSpecified;
			}
			set
			{
				this.canDeleteFieldSpecified = value;
			}
		}

		public bool CanRenameOrMove
		{
			get
			{
				return this.canRenameOrMoveField;
			}
			set
			{
				this.canRenameOrMoveField = value;
			}
		}

		[XmlIgnore]
		public bool CanRenameOrMoveSpecified
		{
			get
			{
				return this.canRenameOrMoveFieldSpecified;
			}
			set
			{
				this.canRenameOrMoveFieldSpecified = value;
			}
		}

		public bool MustDisplayComment
		{
			get
			{
				return this.mustDisplayCommentField;
			}
			set
			{
				this.mustDisplayCommentField = value;
			}
		}

		[XmlIgnore]
		public bool MustDisplayCommentSpecified
		{
			get
			{
				return this.mustDisplayCommentFieldSpecified;
			}
			set
			{
				this.mustDisplayCommentFieldSpecified = value;
			}
		}

		public bool HasQuota
		{
			get
			{
				return this.hasQuotaField;
			}
			set
			{
				this.hasQuotaField = value;
			}
		}

		[XmlIgnore]
		public bool HasQuotaSpecified
		{
			get
			{
				return this.hasQuotaFieldSpecified;
			}
			set
			{
				this.hasQuotaFieldSpecified = value;
			}
		}

		public bool IsManagedFoldersRoot
		{
			get
			{
				return this.isManagedFoldersRootField;
			}
			set
			{
				this.isManagedFoldersRootField = value;
			}
		}

		[XmlIgnore]
		public bool IsManagedFoldersRootSpecified
		{
			get
			{
				return this.isManagedFoldersRootFieldSpecified;
			}
			set
			{
				this.isManagedFoldersRootFieldSpecified = value;
			}
		}

		public string ManagedFolderId
		{
			get
			{
				return this.managedFolderIdField;
			}
			set
			{
				this.managedFolderIdField = value;
			}
		}

		public string Comment
		{
			get
			{
				return this.commentField;
			}
			set
			{
				this.commentField = value;
			}
		}

		public int StorageQuota
		{
			get
			{
				return this.storageQuotaField;
			}
			set
			{
				this.storageQuotaField = value;
			}
		}

		[XmlIgnore]
		public bool StorageQuotaSpecified
		{
			get
			{
				return this.storageQuotaFieldSpecified;
			}
			set
			{
				this.storageQuotaFieldSpecified = value;
			}
		}

		public int FolderSize
		{
			get
			{
				return this.folderSizeField;
			}
			set
			{
				this.folderSizeField = value;
			}
		}

		[XmlIgnore]
		public bool FolderSizeSpecified
		{
			get
			{
				return this.folderSizeFieldSpecified;
			}
			set
			{
				this.folderSizeFieldSpecified = value;
			}
		}

		public string HomePage
		{
			get
			{
				return this.homePageField;
			}
			set
			{
				this.homePageField = value;
			}
		}

		private bool canDeleteField;

		private bool canDeleteFieldSpecified;

		private bool canRenameOrMoveField;

		private bool canRenameOrMoveFieldSpecified;

		private bool mustDisplayCommentField;

		private bool mustDisplayCommentFieldSpecified;

		private bool hasQuotaField;

		private bool hasQuotaFieldSpecified;

		private bool isManagedFoldersRootField;

		private bool isManagedFoldersRootFieldSpecified;

		private string managedFolderIdField;

		private string commentField;

		private int storageQuotaField;

		private bool storageQuotaFieldSpecified;

		private int folderSizeField;

		private bool folderSizeFieldSpecified;

		private string homePageField;
	}
}
