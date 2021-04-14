using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidRootFolderMappingInCSVException : MigrationPermanentException
	{
		public InvalidRootFolderMappingInCSVException(int rowIndex, string folderPath, string identifier, string hierarchyMailboxName) : base(Strings.InvalidRootFolderMappingInCSVError(rowIndex, folderPath, identifier, hierarchyMailboxName))
		{
			this.rowIndex = rowIndex;
			this.folderPath = folderPath;
			this.identifier = identifier;
			this.hierarchyMailboxName = hierarchyMailboxName;
		}

		public InvalidRootFolderMappingInCSVException(int rowIndex, string folderPath, string identifier, string hierarchyMailboxName, Exception innerException) : base(Strings.InvalidRootFolderMappingInCSVError(rowIndex, folderPath, identifier, hierarchyMailboxName), innerException)
		{
			this.rowIndex = rowIndex;
			this.folderPath = folderPath;
			this.identifier = identifier;
			this.hierarchyMailboxName = hierarchyMailboxName;
		}

		protected InvalidRootFolderMappingInCSVException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rowIndex = (int)info.GetValue("rowIndex", typeof(int));
			this.folderPath = (string)info.GetValue("folderPath", typeof(string));
			this.identifier = (string)info.GetValue("identifier", typeof(string));
			this.hierarchyMailboxName = (string)info.GetValue("hierarchyMailboxName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rowIndex", this.rowIndex);
			info.AddValue("folderPath", this.folderPath);
			info.AddValue("identifier", this.identifier);
			info.AddValue("hierarchyMailboxName", this.hierarchyMailboxName);
		}

		public int RowIndex
		{
			get
			{
				return this.rowIndex;
			}
		}

		public string FolderPath
		{
			get
			{
				return this.folderPath;
			}
		}

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public string HierarchyMailboxName
		{
			get
			{
				return this.hierarchyMailboxName;
			}
		}

		private readonly int rowIndex;

		private readonly string folderPath;

		private readonly string identifier;

		private readonly string hierarchyMailboxName;
	}
}
