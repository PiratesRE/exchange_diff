using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Folder : Entity
	{
		public string ParentFolderId
		{
			get
			{
				return (string)base[FolderSchema.ParentFolderId];
			}
			set
			{
				base[FolderSchema.ParentFolderId] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)base[FolderSchema.DisplayName];
			}
			set
			{
				base[FolderSchema.DisplayName] = value;
			}
		}

		public string ClassName
		{
			get
			{
				return (string)base[FolderSchema.ClassName];
			}
			set
			{
				base[FolderSchema.ClassName] = value;
			}
		}

		public int TotalCount
		{
			get
			{
				return (int)base[FolderSchema.TotalCount];
			}
			set
			{
				base[FolderSchema.TotalCount] = value;
			}
		}

		public int ChildFolderCount
		{
			get
			{
				return (int)base[FolderSchema.ChildFolderCount];
			}
			set
			{
				base[FolderSchema.ChildFolderCount] = value;
			}
		}

		public int UnreadItemCount
		{
			get
			{
				return (int)base[FolderSchema.UnreadItemCount];
			}
			set
			{
				base[FolderSchema.UnreadItemCount] = value;
			}
		}

		public IEnumerable<Folder> ChildFolders
		{
			get
			{
				return (IEnumerable<Folder>)base[FolderSchema.ChildFolders];
			}
			set
			{
				base[FolderSchema.ChildFolders] = value;
			}
		}

		public IEnumerable<Message> Messages
		{
			get
			{
				return (IEnumerable<Message>)base[FolderSchema.Messages];
			}
			set
			{
				base[FolderSchema.Messages] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return FolderSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Folder).Namespace, typeof(Folder).Name, Entity.EdmEntityType);
	}
}
