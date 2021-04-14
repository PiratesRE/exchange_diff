using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ManagedFolderInformationType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public bool CanDelete
		{
			get
			{
				return this.canDelete;
			}
			set
			{
				this.CanDeleteSpecified = true;
				this.canDelete = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool CanDeleteSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public bool CanRenameOrMove
		{
			get
			{
				return this.canRenameOrMove;
			}
			set
			{
				this.CanRenameOrMoveSpecified = true;
				this.canRenameOrMove = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool CanRenameOrMoveSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public bool MustDisplayComment
		{
			get
			{
				return this.mustDisplayComment;
			}
			set
			{
				this.MustDisplayCommentSpecified = true;
				this.mustDisplayComment = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MustDisplayCommentSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public bool HasQuota
		{
			get
			{
				return this.hasQuota;
			}
			set
			{
				this.HasQuotaSpecified = true;
				this.hasQuota = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool HasQuotaSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public bool IsManagedFoldersRoot
		{
			get
			{
				return this.isManagedFoldersRoot;
			}
			set
			{
				this.IsManagedFoldersRootSpecified = true;
				this.isManagedFoldersRoot = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsManagedFoldersRootSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string ManagedFolderId { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public string Comment { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public int StorageQuota
		{
			get
			{
				return this.storageQuota;
			}
			set
			{
				this.StorageQuotaSpecified = true;
				this.storageQuota = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool StorageQuotaSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public int FolderSize
		{
			get
			{
				return this.folderSize;
			}
			set
			{
				this.FolderSizeSpecified = true;
				this.folderSize = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool FolderSizeSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 10)]
		public string HomePage { get; set; }

		private bool canDelete;

		private bool canRenameOrMove;

		private bool mustDisplayComment;

		private bool hasQuota;

		private bool isManagedFoldersRoot;

		private int storageQuota;

		private int folderSize;
	}
}
