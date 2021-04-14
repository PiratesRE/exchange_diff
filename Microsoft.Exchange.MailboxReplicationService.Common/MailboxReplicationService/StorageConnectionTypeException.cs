using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StorageConnectionTypeException : MailboxReplicationPermanentException
	{
		public StorageConnectionTypeException(string type) : base(MrsStrings.StorageConnectionType(type))
		{
			this.type = type;
		}

		public StorageConnectionTypeException(string type, Exception innerException) : base(MrsStrings.StorageConnectionType(type), innerException)
		{
			this.type = type;
		}

		protected StorageConnectionTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string type;
	}
}
