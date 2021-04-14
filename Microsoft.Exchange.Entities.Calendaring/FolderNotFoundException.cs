using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FolderNotFoundException : StoragePermanentException
	{
		public FolderNotFoundException(string folderType) : base(CalendaringStrings.FolderNotFound(folderType))
		{
			this.folderType = folderType;
		}

		public FolderNotFoundException(string folderType, Exception innerException) : base(CalendaringStrings.FolderNotFound(folderType), innerException)
		{
			this.folderType = folderType;
		}

		protected FolderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderType = (string)info.GetValue("folderType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderType", this.folderType);
		}

		public string FolderType
		{
			get
			{
				return this.folderType;
			}
		}

		private readonly string folderType;
	}
}
