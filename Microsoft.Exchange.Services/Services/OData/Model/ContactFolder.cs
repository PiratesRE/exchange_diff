using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ContactFolder : Entity
	{
		public string ParentFolderId
		{
			get
			{
				return (string)base[ContactFolderSchema.ParentFolderId];
			}
			set
			{
				base[ContactFolderSchema.ParentFolderId] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)base[ContactFolderSchema.DisplayName];
			}
			set
			{
				base[ContactFolderSchema.DisplayName] = value;
			}
		}

		public IEnumerable<ContactFolder> ChildFolders
		{
			get
			{
				return (IEnumerable<ContactFolder>)base[ContactFolderSchema.ChildFolders];
			}
			set
			{
				base[ContactFolderSchema.ChildFolders] = value;
			}
		}

		public IEnumerable<Contact> Contacts
		{
			get
			{
				return (IEnumerable<Contact>)base[ContactFolderSchema.ChildFolders];
			}
			set
			{
				base[ContactFolderSchema.ChildFolders] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return ContactFolderSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(ContactFolder).Namespace, typeof(ContactFolder).Name, Entity.EdmEntityType);
	}
}
