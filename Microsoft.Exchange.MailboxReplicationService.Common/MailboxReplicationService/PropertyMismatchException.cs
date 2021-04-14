using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PropertyMismatchException : MailboxReplicationPermanentException
	{
		public PropertyMismatchException(uint propTag, string value1, string value2) : base(MrsStrings.PropertyMismatch(propTag, value1, value2))
		{
			this.propTag = propTag;
			this.value1 = value1;
			this.value2 = value2;
		}

		public PropertyMismatchException(uint propTag, string value1, string value2, Exception innerException) : base(MrsStrings.PropertyMismatch(propTag, value1, value2), innerException)
		{
			this.propTag = propTag;
			this.value1 = value1;
			this.value2 = value2;
		}

		protected PropertyMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propTag = (uint)info.GetValue("propTag", typeof(uint));
			this.value1 = (string)info.GetValue("value1", typeof(string));
			this.value2 = (string)info.GetValue("value2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propTag", this.propTag);
			info.AddValue("value1", this.value1);
			info.AddValue("value2", this.value2);
		}

		public uint PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public string Value1
		{
			get
			{
				return this.value1;
			}
		}

		public string Value2
		{
			get
			{
				return this.value2;
			}
		}

		private readonly uint propTag;

		private readonly string value1;

		private readonly string value2;
	}
}
