using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToReadPSTFolderTransientException : MailboxReplicationTransientException
	{
		public UnableToReadPSTFolderTransientException(uint folderId) : base(MrsStrings.UnableToReadPSTFolder(folderId))
		{
			this.folderId = folderId;
		}

		public UnableToReadPSTFolderTransientException(uint folderId, Exception innerException) : base(MrsStrings.UnableToReadPSTFolder(folderId), innerException)
		{
			this.folderId = folderId;
		}

		protected UnableToReadPSTFolderTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderId = (uint)info.GetValue("folderId", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderId", this.folderId);
		}

		public uint FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		private readonly uint folderId;
	}
}
