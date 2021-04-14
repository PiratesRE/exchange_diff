using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class AppendToFolderFieldType : FolderChangeDescriptionType
	{
		[XmlElement("TasksFolder", typeof(TasksFolderType))]
		[XmlElement("CalendarFolder", typeof(CalendarFolderType))]
		[XmlElement("ContactsFolder", typeof(ContactsFolderType))]
		[XmlElement("Folder", typeof(FolderType))]
		[XmlElement("SearchFolder", typeof(SearchFolderType))]
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
