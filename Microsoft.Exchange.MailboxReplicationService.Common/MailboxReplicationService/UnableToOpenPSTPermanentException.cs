using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToOpenPSTPermanentException : MailboxReplicationPermanentException
	{
		public UnableToOpenPSTPermanentException(string filePath, string exceptionMessage) : base(MrsStrings.UnableToOpenPST2(filePath, exceptionMessage))
		{
			this.filePath = filePath;
			this.exceptionMessage = exceptionMessage;
		}

		public UnableToOpenPSTPermanentException(string filePath, string exceptionMessage, Exception innerException) : base(MrsStrings.UnableToOpenPST2(filePath, exceptionMessage), innerException)
		{
			this.filePath = filePath;
			this.exceptionMessage = exceptionMessage;
		}

		protected UnableToOpenPSTPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filePath = (string)info.GetValue("filePath", typeof(string));
			this.exceptionMessage = (string)info.GetValue("exceptionMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filePath", this.filePath);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		public string ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly string filePath;

		private readonly string exceptionMessage;
	}
}
