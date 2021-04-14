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
	internal class DuplicateFolderInCSVException : MigrationPermanentException
	{
		public DuplicateFolderInCSVException(int rowIndex, string folderPath, string identifier) : base(Strings.DuplicateFolderInCSVError(rowIndex, folderPath, identifier))
		{
			this.rowIndex = rowIndex;
			this.folderPath = folderPath;
			this.identifier = identifier;
		}

		public DuplicateFolderInCSVException(int rowIndex, string folderPath, string identifier, Exception innerException) : base(Strings.DuplicateFolderInCSVError(rowIndex, folderPath, identifier), innerException)
		{
			this.rowIndex = rowIndex;
			this.folderPath = folderPath;
			this.identifier = identifier;
		}

		protected DuplicateFolderInCSVException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rowIndex = (int)info.GetValue("rowIndex", typeof(int));
			this.folderPath = (string)info.GetValue("folderPath", typeof(string));
			this.identifier = (string)info.GetValue("identifier", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rowIndex", this.rowIndex);
			info.AddValue("folderPath", this.folderPath);
			info.AddValue("identifier", this.identifier);
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

		private readonly int rowIndex;

		private readonly string folderPath;

		private readonly string identifier;
	}
}
