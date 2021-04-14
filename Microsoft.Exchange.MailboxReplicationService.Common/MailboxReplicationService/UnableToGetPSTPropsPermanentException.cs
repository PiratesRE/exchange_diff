using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToGetPSTPropsPermanentException : MailboxReplicationPermanentException
	{
		public UnableToGetPSTPropsPermanentException(string filePath) : base(MrsStrings.UnableToGetPSTProps(filePath))
		{
			this.filePath = filePath;
		}

		public UnableToGetPSTPropsPermanentException(string filePath, Exception innerException) : base(MrsStrings.UnableToGetPSTProps(filePath), innerException)
		{
			this.filePath = filePath;
		}

		protected UnableToGetPSTPropsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
