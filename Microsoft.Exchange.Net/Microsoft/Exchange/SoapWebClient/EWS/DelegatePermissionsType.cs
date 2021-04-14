using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DelegatePermissionsType
	{
		public DelegateFolderPermissionLevelType CalendarFolderPermissionLevel;

		[XmlIgnore]
		public bool CalendarFolderPermissionLevelSpecified;

		public DelegateFolderPermissionLevelType TasksFolderPermissionLevel;

		[XmlIgnore]
		public bool TasksFolderPermissionLevelSpecified;

		public DelegateFolderPermissionLevelType InboxFolderPermissionLevel;

		[XmlIgnore]
		public bool InboxFolderPermissionLevelSpecified;

		public DelegateFolderPermissionLevelType ContactsFolderPermissionLevel;

		[XmlIgnore]
		public bool ContactsFolderPermissionLevelSpecified;

		public DelegateFolderPermissionLevelType NotesFolderPermissionLevel;

		[XmlIgnore]
		public bool NotesFolderPermissionLevelSpecified;

		public DelegateFolderPermissionLevelType JournalFolderPermissionLevel;

		[XmlIgnore]
		public bool JournalFolderPermissionLevelSpecified;
	}
}
