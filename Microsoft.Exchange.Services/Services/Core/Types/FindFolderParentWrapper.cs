using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindFolderParentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FindFolderParentWrapper : FindParentWrapperBase
	{
		public FindFolderParentWrapper()
		{
		}

		internal FindFolderParentWrapper(BaseFolderType[] folders, BaseFolderType parentFolder, BasePageResult paging) : base(paging)
		{
			this.Folders = folders;
			this.ParentFolder = parentFolder;
		}

		[XmlArrayItem("Folder", typeof(FolderType), IsNullable = false)]
		[XmlArrayItem("SearchFolder", typeof(SearchFolderType), IsNullable = false)]
		[XmlArrayItem("ContactsFolder", typeof(ContactsFolderType), IsNullable = false)]
		[DataMember(IsRequired = true)]
		[XmlArrayItem("CalendarFolder", typeof(CalendarFolderType), IsNullable = false)]
		[XmlArrayItem("TasksFolder", typeof(TasksFolderType), IsNullable = false)]
		public BaseFolderType[] Folders { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public BaseFolderType ParentFolder { get; set; }
	}
}
