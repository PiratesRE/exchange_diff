using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderLocationUnknownException : LocalizedException
	{
		public FolderLocationUnknownException(string folder) : base(Strings.ErrorFolderLocationUnknown(folder))
		{
			this.folder = folder;
		}

		public FolderLocationUnknownException(string folder, Exception innerException) : base(Strings.ErrorFolderLocationUnknown(folder), innerException)
		{
			this.folder = folder;
		}

		protected FolderLocationUnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
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
