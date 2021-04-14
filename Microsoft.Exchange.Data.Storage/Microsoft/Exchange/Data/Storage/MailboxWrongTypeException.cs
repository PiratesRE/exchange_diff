using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxWrongTypeException : MailboxNotFoundException
	{
		public MailboxWrongTypeException(string externalDirectoryObjectId, string type) : base(ServerStrings.InvalidMailboxType(externalDirectoryObjectId, type))
		{
			this.externalDirectoryObjectId = externalDirectoryObjectId;
			this.type = type;
		}

		public MailboxWrongTypeException(string externalDirectoryObjectId, string type, Exception innerException) : base(ServerStrings.InvalidMailboxType(externalDirectoryObjectId, type), innerException)
		{
			this.externalDirectoryObjectId = externalDirectoryObjectId;
			this.type = type;
		}

		protected MailboxWrongTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.externalDirectoryObjectId = (string)info.GetValue("externalDirectoryObjectId", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("externalDirectoryObjectId", this.externalDirectoryObjectId);
			info.AddValue("type", this.type);
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return this.externalDirectoryObjectId;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string externalDirectoryObjectId;

		private readonly string type;
	}
}
