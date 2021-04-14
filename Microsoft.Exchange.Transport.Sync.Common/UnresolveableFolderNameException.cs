using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnresolveableFolderNameException : LocalizedException
	{
		public UnresolveableFolderNameException(string folderName) : base(Strings.UnresolveableFolderNameException(folderName))
		{
			this.folderName = folderName;
		}

		public UnresolveableFolderNameException(string folderName, Exception innerException) : base(Strings.UnresolveableFolderNameException(folderName), innerException)
		{
			this.folderName = folderName;
		}

		protected UnresolveableFolderNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderName = (string)info.GetValue("folderName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderName", this.folderName);
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		private readonly string folderName;
	}
}
