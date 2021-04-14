using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class SetFolderFieldType : FolderChangeDescriptionType
	{
		[XmlElement("SearchFolder", typeof(SearchFolderType))]
		[XmlElement("Folder", typeof(FolderType))]
		[XmlElement("TasksFolder", typeof(TasksFolderType))]
		[XmlElement("ContactsFolder", typeof(ContactsFolderType))]
		[XmlElement("CalendarFolder", typeof(CalendarFolderType))]
		public BaseFolderType Item1
		{
			get
			{
				return this.item1Field;
			}
			set
			{
				this.item1Field = value;
			}
		}

		private BaseFolderType item1Field;
	}
}
