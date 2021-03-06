using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptNamedPropDataException : MailboxReplicationPermanentException
	{
		public CorruptNamedPropDataException(string type) : base(MrsStrings.CorruptNamedPropData(type))
		{
			this.type = type;
		}

		public CorruptNamedPropDataException(string type, Exception innerException) : base(MrsStrings.CorruptNamedPropData(type), innerException)
		{
			this.type = type;
		}

		protected CorruptNamedPropDataException(SerializationInfo info, StreamingContext context) : base(info, context)
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
