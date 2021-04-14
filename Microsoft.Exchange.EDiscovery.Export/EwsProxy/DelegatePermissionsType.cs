using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class DelegatePermissionsType
	{
		public DelegateFolderPermissionLevelType CalendarFolderPermissionLevel
		{
			get
			{
				return this.calendarFolderPermissionLevelField;
			}
			set
			{
				this.calendarFolderPermissionLevelField = value;
			}
		}

		[XmlIgnore]
		public bool CalendarFolderPermissionLevelSpecified
		{
			get
			{
				return this.calendarFolderPermissionLevelFieldSpecified;
			}
			set
			{
				this.calendarFolderPermissionLevelFieldSpecified = value;
			}
		}

		public DelegateFolderPermissionLevelType TasksFolderPermissionLevel
		{
			get
			{
				return this.tasksFolderPermissionLevelField;
			}
			set
			{
				this.tasksFolderPermissionLevelField = value;
			}
		}

		[XmlIgnore]
		public bool TasksFolderPermissionLevelSpecified
		{
			get
			{
				return this.tasksFolderPermissionLevelFieldSpecified;
			}
			set
			{
				this.tasksFolderPermissionLevelFieldSpecified = value;
			}
		}

		public DelegateFolderPermissionLevelType InboxFolderPermissionLevel
		{
			get
			{
				return this.inboxFolderPermissionLevelField;
			}
			set
			{
				this.inboxFolderPermissionLevelField = value;
			}
		}

		[XmlIgnore]
		public bool InboxFolderPermissionLevelSpecified
		{
			get
			{
				return this.inboxFolderPermissionLevelFieldSpecified;
			}
			set
			{
				this.inboxFolderPermissionLevelFieldSpecified = value;
			}
		}

		public DelegateFolderPermissionLevelType ContactsFolderPermissionLevel
		{
			get
			{
				return this.contactsFolderPermissionLevelField;
			}
			set
			{
				this.contactsFolderPermissionLevelField = value;
			}
		}

		[XmlIgnore]
		public bool ContactsFolderPermissionLevelSpecified
		{
			get
			{
				return this.contactsFolderPermissionLevelFieldSpecified;
			}
			set
			{
				this.contactsFolderPermissionLevelFieldSpecified = value;
			}
		}

		public DelegateFolderPermissionLevelType NotesFolderPermissionLevel
		{
			get
			{
				return this.notesFolderPermissionLevelField;
			}
			set
			{
				this.notesFolderPermissionLevelField = value;
			}
		}

		[XmlIgnore]
		public bool NotesFolderPermissionLevelSpecified
		{
			get
			{
				return this.notesFolderPermissionLevelFieldSpecified;
			}
			set
			{
				this.notesFolderPermissionLevelFieldSpecified = value;
			}
		}

		public DelegateFolderPermissionLevelType JournalFolderPermissionLevel
		{
			get
			{
				return this.journalFolderPermissionLevelField;
			}
			set
			{
				this.journalFolderPermissionLevelField = value;
			}
		}

		[XmlIgnore]
		public bool JournalFolderPermissionLevelSpecified
		{
			get
			{
				return this.journalFolderPermissionLevelFieldSpecified;
			}
			set
			{
				this.journalFolderPermissionLevelFieldSpecified = value;
			}
		}

		private DelegateFolderPermissionLevelType calendarFolderPermissionLevelField;

		private bool calendarFolderPermissionLevelFieldSpecified;

		private DelegateFolderPermissionLevelType tasksFolderPermissionLevelField;

		private bool tasksFolderPermissionLevelFieldSpecified;

		private DelegateFolderPermissionLevelType inboxFolderPermissionLevelField;

		private bool inboxFolderPermissionLevelFieldSpecified;

		private DelegateFolderPermissionLevelType contactsFolderPermissionLevelField;

		private bool contactsFolderPermissionLevelFieldSpecified;

		private DelegateFolderPermissionLevelType notesFolderPermissionLevelField;

		private bool notesFolderPermissionLevelFieldSpecified;

		private DelegateFolderPermissionLevelType journalFolderPermissionLevelField;

		private bool journalFolderPermissionLevelFieldSpecified;
	}
}
