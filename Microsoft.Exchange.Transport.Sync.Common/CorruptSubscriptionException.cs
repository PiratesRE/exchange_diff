using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CorruptSubscriptionException : LocalizedException
	{
		public CorruptSubscriptionException(Guid guid) : base(Strings.CorruptSubscriptionException(guid))
		{
			this.guid = guid;
		}

		public CorruptSubscriptionException(Guid guid, Exception innerException) : base(Strings.CorruptSubscriptionException(guid), innerException)
		{
			this.guid = guid;
		}

		protected CorruptSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (Guid)info.GetValue("guid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		private readonly Guid guid;
	}
}
