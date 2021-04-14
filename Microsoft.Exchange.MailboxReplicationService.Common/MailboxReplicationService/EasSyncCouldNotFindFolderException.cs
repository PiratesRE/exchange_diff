using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasSyncCouldNotFindFolderException : MailboxReplicationTransientException
	{
		public EasSyncCouldNotFindFolderException(string folderId) : base(MrsStrings.EasSyncCouldNotFindFolder(folderId))
		{
			this.folderId = folderId;
		}

		public EasSyncCouldNotFindFolderException(string folderId, Exception innerException) : base(MrsStrings.EasSyncCouldNotFindFolder(folderId), innerException)
		{
			this.folderId = folderId;
		}

		protected EasSyncCouldNotFindFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
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
