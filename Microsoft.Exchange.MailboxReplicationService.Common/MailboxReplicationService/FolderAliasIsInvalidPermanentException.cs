using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderAliasIsInvalidPermanentException : FolderFilterPermanentException
	{
		public FolderAliasIsInvalidPermanentException(string folderAlias) : base(MrsStrings.FolderAliasIsInvalid(folderAlias))
		{
			this.folderAlias = folderAlias;
		}

		public FolderAliasIsInvalidPermanentException(string folderAlias, Exception innerException) : base(MrsStrings.FolderAliasIsInvalid(folderAlias), innerException)
		{
			this.folderAlias = folderAlias;
		}

		protected FolderAliasIsInvalidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderAlias = (string)info.GetValue("folderAlias", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderAlias", this.folderAlias);
		}

		public string FolderAlias
		{
			get
			{
				return this.folderAlias;
			}
		}

		private readonly string folderAlias;
	}
}
