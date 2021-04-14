using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FastTransferBufferException : MailboxReplicationPermanentException
	{
		public FastTransferBufferException(string property, int value) : base(MrsStrings.FastTransferBuffer(property, value))
		{
			this.property = property;
			this.value = value;
		}

		public FastTransferBufferException(string property, int value, Exception innerException) : base(MrsStrings.FastTransferBuffer(property, value), innerException)
		{
			this.property = property;
			this.value = value;
		}

		protected FastTransferBufferException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
			this.value = (int)info.GetValue("value", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
			info.AddValue("value", this.value);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string property;

		private readonly int value;
	}
}
