using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "AppendToFolderField", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "AppendToFolderFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class AppendFolderPropertyUpdate : AppendPropertyUpdate
	{
		[XmlElement("Folder", typeof(FolderType))]
		[XmlElement("ContactsFolder", typeof(ContactsFolderType))]
		[DataMember(IsRequired = true)]
		[XmlElement("SearchFolder", typeof(SearchFolderType))]
		[XmlElement("TasksFolder", typeof(TasksFolderType))]
		[XmlElement("CalendarFolder", typeof(CalendarFolderType))]
		public BaseFolderType Folder { get; set; }

		internal override ServiceObject ServiceObject
		{
			get
			{
				return this.Folder;
			}
		}
	}
}
