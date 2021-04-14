using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderPathIsInvalidPermanentException : FolderFilterPermanentException
	{
		public FolderPathIsInvalidPermanentException(string folderPath) : base(MrsStrings.FolderPathIsInvalid(folderPath))
		{
			this.folderPath = folderPath;
		}

		public FolderPathIsInvalidPermanentException(string folderPath, Exception innerException) : base(MrsStrings.FolderPathIsInvalid(folderPath), innerException)
		{
			this.folderPath = folderPath;
		}

		protected FolderPathIsInvalidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderPath = (string)info.GetValue("folderPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderPath", this.folderPath);
		}

		public string FolderPath
		{
			get
			{
				return this.folderPath;
			}
		}

		private readonly string folderPath;
	}
}
