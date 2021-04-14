using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class ListItemWrapper : ClientObjectWrapper<ListItem>, IListItem, IClientObject<ListItem>
	{
		public object this[string fieldName]
		{
			get
			{
				return this.backingItem[fieldName];
			}
			set
			{
				this.backingItem[fieldName] = value;
			}
		}

		public int Id
		{
			get
			{
				return this.backingItem.Id;
			}
		}

		public string IdAsString
		{
			get
			{
				return this.backingItem.Id.ToString();
			}
		}

		public IFile File
		{
			get
			{
				IFile result;
				if ((result = this.file) == null)
				{
					result = (this.file = new FileWrapper(this.backingItem.File));
				}
				return result;
			}
		}

		public IFolder Folder
		{
			get
			{
				IFolder result;
				if ((result = this.folder) == null)
				{
					result = (this.folder = new FolderWrapper(this.backingItem.Folder));
				}
				return result;
			}
		}

		public void BreakRoleInheritance(bool copyRoleAssignments, bool clearSubscopes)
		{
			this.backingItem.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
		}

		public ListItemWrapper(ListItem item) : base(item)
		{
			this.backingItem = item;
		}

		private ListItem backingItem;

		private IFile file;

		private IFolder folder;
	}
}
