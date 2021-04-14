using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToStreamPSTPropPermanentException : MailboxReplicationPermanentException
	{
		public UnableToStreamPSTPropPermanentException(uint propTag, int offset, int bytesToRead, long length) : base(MrsStrings.UnableToStreamPSTProp(propTag, offset, bytesToRead, length))
		{
			this.propTag = propTag;
			this.offset = offset;
			this.bytesToRead = bytesToRead;
			this.length = length;
		}

		public UnableToStreamPSTPropPermanentException(uint propTag, int offset, int bytesToRead, long length, Exception innerException) : base(MrsStrings.UnableToStreamPSTProp(propTag, offset, bytesToRead, length), innerException)
		{
			this.propTag = propTag;
			this.offset = offset;
			this.bytesToRead = bytesToRead;
			this.length = length;
		}

		protected UnableToStreamPSTPropPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propTag = (uint)info.GetValue("propTag", typeof(uint));
			this.offset = (int)info.GetValue("offset", typeof(int));
			this.bytesToRead = (int)info.GetValue("bytesToRead", typeof(int));
			this.length = (long)info.GetValue("length", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propTag", this.propTag);
			info.AddValue("offset", this.offset);
			info.AddValue("bytesToRead", this.bytesToRead);
			info.AddValue("length", this.length);
		}

		public uint PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public int BytesToRead
		{
			get
			{
				return this.bytesToRead;
			}
		}

		public long Length
		{
			get
			{
				return this.length;
			}
		}

		private readonly uint propTag;

		private readonly int offset;

		private readonly int bytesToRead;

		private readonly long length;
	}
}
