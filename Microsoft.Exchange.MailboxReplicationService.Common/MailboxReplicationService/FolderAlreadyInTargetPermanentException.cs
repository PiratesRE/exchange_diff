using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderAlreadyInTargetPermanentException : MailboxReplicationPermanentException
	{
		public FolderAlreadyInTargetPermanentException(string folderId) : base(MrsStrings.FolderAlreadyInTarget(folderId))
		{
			this.folderId = folderId;
		}

		public FolderAlreadyInTargetPermanentException(string folderId, Exception innerException) : base(MrsStrings.FolderAlreadyInTarget(folderId), innerException)
		{
			this.folderId = folderId;
		}

		protected FolderAlreadyInTargetPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderId = (string)info.GetValue("folderId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderId", this.folderId);
		}

		public string FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		private readonly string folderId;
	}
}
