using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToGetPSTFolderPropsTransientException : MailboxReplicationTransientException
	{
		public UnableToGetPSTFolderPropsTransientException(uint folderId) : base(MrsStrings.UnableToGetPSTFolderProps(folderId))
		{
			this.folderId = folderId;
		}

		public UnableToGetPSTFolderPropsTransientException(uint folderId, Exception innerException) : base(MrsStrings.UnableToGetPSTFolderProps(folderId), innerException)
		{
			this.folderId = folderId;
		}

		protected UnableToGetPSTFolderPropsTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
