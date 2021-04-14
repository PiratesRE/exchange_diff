using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToLoadPSTSyncStatePermanentException : MailboxReplicationPermanentException
	{
		public UnableToLoadPSTSyncStatePermanentException(string filePath) : base(MrsStrings.UnableToLoadPSTSyncState(filePath))
		{
			this.filePath = filePath;
		}

		public UnableToLoadPSTSyncStatePermanentException(string filePath, Exception innerException) : base(MrsStrings.UnableToLoadPSTSyncState(filePath), innerException)
		{
			this.filePath = filePath;
		}

		protected UnableToLoadPSTSyncStatePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filePath = (string)info.GetValue("filePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filePath", this.filePath);
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		private readonly string filePath;
	}
}
