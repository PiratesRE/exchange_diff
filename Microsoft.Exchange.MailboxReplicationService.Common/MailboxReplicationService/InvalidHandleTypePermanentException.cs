using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidHandleTypePermanentException : MailboxReplicationPermanentException
	{
		public InvalidHandleTypePermanentException(long handle, string handleType, string expectedType) : base(MrsStrings.InvalidHandleType(handle, handleType, expectedType))
		{
			this.handle = handle;
			this.handleType = handleType;
			this.expectedType = expectedType;
		}

		public InvalidHandleTypePermanentException(long handle, string handleType, string expectedType, Exception innerException) : base(MrsStrings.InvalidHandleType(handle, handleType, expectedType), innerException)
		{
			this.handle = handle;
			this.handleType = handleType;
			this.expectedType = expectedType;
		}

		protected InvalidHandleTypePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.handle = (long)info.GetValue("handle", typeof(long));
			this.handleType = (string)info.GetValue("handleType", typeof(string));
			this.expectedType = (string)info.GetValue("expectedType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("handle", this.handle);
			info.AddValue("handleType", this.handleType);
			info.AddValue("expectedType", this.expectedType);
		}

		public long Handle
		{
			get
			{
				return this.handle;
			}
		}

		public string HandleType
		{
			get
			{
				return this.handleType;
			}
		}

		public string ExpectedType
		{
			get
			{
				return this.expectedType;
			}
		}

		private readonly long handle;

		private readonly string handleType;

		private readonly string expectedType;
	}
}
