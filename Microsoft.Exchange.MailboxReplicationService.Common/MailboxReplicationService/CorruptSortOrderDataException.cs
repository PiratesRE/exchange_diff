using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptSortOrderDataException : MailboxReplicationPermanentException
	{
		public CorruptSortOrderDataException(int flags) : base(MrsStrings.CorruptSortOrderData(flags))
		{
			this.flags = flags;
		}

		public CorruptSortOrderDataException(int flags, Exception innerException) : base(MrsStrings.CorruptSortOrderData(flags), innerException)
		{
			this.flags = flags;
		}

		protected CorruptSortOrderDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.flags = (int)info.GetValue("flags", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("flags", this.flags);
		}

		public int Flags
		{
			get
			{
				return this.flags;
			}
		}

		private readonly int flags;
	}
}
