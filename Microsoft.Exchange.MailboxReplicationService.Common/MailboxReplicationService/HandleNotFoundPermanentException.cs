using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HandleNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public HandleNotFoundPermanentException(long handle) : base(MrsStrings.HandleNotFound(handle))
		{
			this.handle = handle;
		}

		public HandleNotFoundPermanentException(long handle, Exception innerException) : base(MrsStrings.HandleNotFound(handle), innerException)
		{
			this.handle = handle;
		}

		protected HandleNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.handle = (long)info.GetValue("handle", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("handle", this.handle);
		}

		public long Handle
		{
			get
			{
				return this.handle;
			}
		}

		private readonly long handle;
	}
}
