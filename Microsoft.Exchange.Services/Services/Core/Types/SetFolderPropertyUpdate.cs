using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SetFolderField", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SetFolderFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class SetFolderPropertyUpdate : SetPropertyUpdate
	{
		[XmlElement("CalendarFolder", typeof(CalendarFolderType))]
		[XmlElement("ContactsFolder", typeof(ContactsFolderType))]
		[XmlElement("Folder", typeof(FolderType))]
		[XmlElement("SearchFolder", typeof(SearchFolderType))]
		[XmlElement("TasksFolder", typeof(TasksFolderType))]
		[DataMember(IsRequired = true)]
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
