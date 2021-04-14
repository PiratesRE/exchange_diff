using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockListItem : MockClientObject<ListItem>, IListItem, IClientObject<ListItem>
	{
		public static MockListItem GetById(string id)
		{
			if (MockListItem.idAndItemMap == null)
			{
				return null;
			}
			MockListItem result;
			if (MockListItem.idAndItemMap.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public object this[string fieldName]
		{
			get
			{
				object result;
				if (!this.values.TryGetValue(fieldName, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				this.values[fieldName] = value;
			}
		}

		public int Id
		{
			get
			{
				return (int)this["ID"];
			}
		}

		public string IdAsString
		{
			get
			{
				return (string)this["FileRef"];
			}
		}

		public IFile File
		{
			get
			{
				if (this["FSObjType"].ToString() == "0")
				{
					return new MockFile(this, this.context);
				}
				return null;
			}
		}

		public IFolder Folder
		{
			get
			{
				if (this["FSObjType"].ToString() == "1")
				{
					return new MockFolder(this, this.context);
				}
				return null;
			}
		}

		public void BreakRoleInheritance(bool copyRoleAssignments, bool clearSubscopes)
		{
		}

		public MockListItem(DirectoryInfo dirInfo, string folderRelativePath, MockClientContext context) : this(context)
		{
			this.values["FSObjType"] = "1";
			this.values["FileLeafRef"] = dirInfo.Name;
			this.values["ID"] = this.GetId(dirInfo.FullName);
			this.values["SortBehavior"] = string.Empty;
			this.values["FileRef"] = Path.Combine(folderRelativePath, dirInfo.Name);
			this.values["File_x0020_Size"] = 0;
			if (dirInfo.Exists)
			{
				this.values["Modified"] = dirInfo.LastWriteTimeUtc;
			}
			this.values["Editor"] = FieldUserValue.FromUser("Administrator");
			this.values["ItemChildCount"] = "0";
			this.values["FolderChildCount"] = "0";
		}

		public MockListItem(FileInfo fileInfo, string folderRelativePath, MockClientContext context) : this(context)
		{
			this.values["FSObjType"] = "0";
			this.values["FileLeafRef"] = fileInfo.Name;
			this.values["ID"] = this.GetId(fileInfo.FullName);
			this.values["SortBehavior"] = string.Empty;
			this.values["FileRef"] = Path.Combine(folderRelativePath, fileInfo.Name);
			if (fileInfo.Exists)
			{
				this.values["File_x0020_Size"] = fileInfo.Length;
				this.values["Modified"] = fileInfo.LastWriteTimeUtc;
			}
			this.values["Editor"] = FieldUserValue.FromUser("Administrator");
			this.values["ItemChildCount"] = "0";
			this.values["FolderChildCount"] = "0";
		}

		private MockListItem(MockClientContext context)
		{
			this.values = new Dictionary<string, object>();
			this.context = context;
		}

		public override void LoadMockData()
		{
		}

		private int GetId(string fullpath)
		{
			if (MockListItem.pathAndIdMap == null)
			{
				MockListItem.pathAndIdMap = new Dictionary<string, int>();
			}
			int result;
			if (MockListItem.pathAndIdMap.TryGetValue(fullpath, out result))
			{
				return result;
			}
			result = (MockListItem.pathAndIdMap[fullpath] = ++MockListItem.idCounter);
			if (MockListItem.idAndItemMap == null)
			{
				MockListItem.idAndItemMap = new Dictionary<string, MockListItem>();
			}
			MockListItem.idAndItemMap[result.ToString()] = this;
			return result;
		}

		private const string DummyEditorName = "Administrator";

		private const string FileObjectType = "0";

		private const string FolderObjectType = "1";

		private static int idCounter = 0;

		private static Dictionary<string, int> pathAndIdMap;

		private static Dictionary<string, MockListItem> idAndItemMap;

		private Dictionary<string, object> values;

		private MockClientContext context;
	}
}
