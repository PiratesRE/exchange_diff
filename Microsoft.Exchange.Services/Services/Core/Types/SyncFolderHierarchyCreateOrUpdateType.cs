using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SyncFolderHierarchyCreateOrUpdateType : SyncFolderHierarchyChangeBase
	{
		public SyncFolderHierarchyCreateOrUpdateType()
		{
		}

		public SyncFolderHierarchyCreateOrUpdateType(BaseFolderType folder, bool isUpdate)
		{
			this.Folder = folder;
			this.isUpdate = isUpdate;
		}

		[XmlElement("SearchFolder", typeof(SearchFolderType))]
		[XmlElement("CalendarFolder", typeof(CalendarFolderType))]
		[DataMember(Name = "Folder", EmitDefaultValue = false)]
		[XmlElement("TasksFolder", typeof(TasksFolderType))]
		[XmlElement("ContactsFolder", typeof(ContactsFolderType))]
		[XmlElement("Folder", typeof(FolderType))]
		public BaseFolderType Folder { get; set; }

		public override SyncFolderHierarchyChangesEnum ChangeType
		{
			get
			{
				if (!this.isUpdate)
				{
					return SyncFolderHierarchyChangesEnum.Create;
				}
				return SyncFolderHierarchyChangesEnum.Update;
			}
		}

		private bool isUpdate;
	}
}
