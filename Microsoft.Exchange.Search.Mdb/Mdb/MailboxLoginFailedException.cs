using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Mdb
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MailboxLoginFailedException : OperationFailedException
	{
		public MailboxLoginFailedException(StoreSessionCacheKey key) : base(Strings.MailboxLoginFailed(key))
		{
			this.key = key;
		}

		public MailboxLoginFailedException(StoreSessionCacheKey key, Exception innerException) : base(Strings.MailboxLoginFailed(key), innerException)
		{
			this.key = key;
		}

		protected MailboxLoginFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.key = (StoreSessionCacheKey)info.GetValue("key", typeof(StoreSessionCacheKey));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("key", this.key);
		}

		public StoreSessionCacheKey Key
		{
			get
			{
				return this.key;
			}
		}

		private readonly StoreSessionCacheKey key;
	}
}
