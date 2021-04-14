using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderNotExistException : LocalizedException
	{
		public FolderNotExistException(string folder) : base(Strings.ErrorFolderNotExist(folder))
		{
			this.folder = folder;
		}

		public FolderNotExistException(string folder, Exception innerException) : base(Strings.ErrorFolderNotExist(folder), innerException)
		{
			this.folder = folder;
		}

		protected FolderNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folder = (string)info.GetValue("folder", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folder", this.folder);
		}

		public string Folder
		{
			get
			{
				return this.folder;
			}
		}

		private readonly string folder;
	}
}
