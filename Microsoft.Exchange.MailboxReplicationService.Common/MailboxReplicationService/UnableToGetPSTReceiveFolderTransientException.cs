using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToGetPSTReceiveFolderTransientException : MailboxReplicationTransientException
	{
		public UnableToGetPSTReceiveFolderTransientException(string filePath, string messageClass) : base(MrsStrings.UnableToGetPSTReceiveFolder(filePath, messageClass))
		{
			this.filePath = filePath;
			this.messageClass = messageClass;
		}

		public UnableToGetPSTReceiveFolderTransientException(string filePath, string messageClass, Exception innerException) : base(MrsStrings.UnableToGetPSTReceiveFolder(filePath, messageClass), innerException)
		{
			this.filePath = filePath;
			this.messageClass = messageClass;
		}

		protected UnableToGetPSTReceiveFolderTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filePath = (string)info.GetValue("filePath", typeof(string));
			this.messageClass = (string)info.GetValue("messageClass", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filePath", this.filePath);
			info.AddValue("messageClass", this.messageClass);
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		private readonly string filePath;

		private readonly string messageClass;
	}
}
