using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DelegatePermissionsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DelegatePermissionsType
	{
		[DefaultValue(DelegateFolderPermissionLevelType.Default)]
		[XmlElement("CalendarFolderPermissionLevel")]
		public DelegateFolderPermissionLevelType CalendarFolderPermissionLevel
		{
			get
			{
				return this.calenderFolderPermission;
			}
			set
			{
				this.calenderFolderPermission = value;
			}
		}

		[DefaultValue(DelegateFolderPermissionLevelType.Default)]
		[XmlElement("TasksFolderPermissionLevel")]
		public DelegateFolderPermissionLevelType TasksFolderPermissionLevel
		{
			get
			{
				return this.tasksFolderPermission;
			}
			set
			{
				this.tasksFolderPermission = value;
			}
		}

		[XmlElement("InboxFolderPermissionLevel")]
		[DefaultValue(DelegateFolderPermissionLevelType.Default)]
		public DelegateFolderPermissionLevelType InboxFolderPermissionLevel
		{
			get
			{
				return this.inboxFolderPermission;
			}
			set
			{
				this.inboxFolderPermission = value;
			}
		}

		[XmlElement("ContactsFolderPermissionLevel")]
		[DefaultValue(DelegateFolderPermissionLevelType.Default)]
		public DelegateFolderPermissionLevelType ContactsFolderPermissionLevel
		{
			get
			{
				return this.contactsFolderPermission;
			}
			set
			{
				this.contactsFolderPermission = value;
			}
		}

		[XmlElement("NotesFolderPermissionLevel")]
		[DefaultValue(DelegateFolderPermissionLevelType.Default)]
		public DelegateFolderPermissionLevelType NotesFolderPermissionLevel
		{
			get
			{
				return this.notesFolderPermission;
			}
			set
			{
				this.notesFolderPermission = value;
			}
		}

		[DefaultValue(DelegateFolderPermissionLevelType.Default)]
		[XmlElement("JournalFolderPermissionLevel")]
		public DelegateFolderPermissionLevelType JournalFolderPermissionLevel
		{
			get
			{
				return this.journalFolderPermission;
			}
			set
			{
				this.journalFolderPermission = value;
			}
		}

		private DelegateFolderPermissionLevelType calenderFolderPermission;

		private DelegateFolderPermissionLevelType tasksFolderPermission;

		private DelegateFolderPermissionLevelType inboxFolderPermission;

		private DelegateFolderPermissionLevelType contactsFolderPermission;

		private DelegateFolderPermissionLevelType notesFolderPermission;

		private DelegateFolderPermissionLevelType journalFolderPermission;
	}
}
