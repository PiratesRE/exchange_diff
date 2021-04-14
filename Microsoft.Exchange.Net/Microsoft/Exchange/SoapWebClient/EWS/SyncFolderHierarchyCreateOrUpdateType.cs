using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class SyncFolderHierarchyCreateOrUpdateType
	{
		[XmlElement("CalendarFolder", typeof(CalendarFolderType))]
		[XmlElement("SearchFolder", typeof(SearchFolderType))]
		[XmlElement("ContactsFolder", typeof(ContactsFolderType))]
		[XmlElement("Folder", typeof(FolderType))]
		[XmlElement("TasksFolder", typeof(TasksFolderType))]
		public BaseFolderType Item;
	}
}
