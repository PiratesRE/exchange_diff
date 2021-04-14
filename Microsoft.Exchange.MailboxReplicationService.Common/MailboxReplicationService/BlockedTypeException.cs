using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BlockedTypeException : MailboxReplicationPermanentException
	{
		public BlockedTypeException(string type) : base(MrsStrings.BlockedType(type))
		{
			this.type = type;
		}

		public BlockedTypeException(string type, Exception innerException) : base(MrsStrings.BlockedType(type), innerException)
		{
			this.type = type;
		}

		protected BlockedTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
