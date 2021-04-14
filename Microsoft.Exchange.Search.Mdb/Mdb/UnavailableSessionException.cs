using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Mdb
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnavailableSessionException : OperationFailedException
	{
		public UnavailableSessionException(StoreSessionCacheKey key) : base(Strings.UnavailableSession(key))
		{
			this.key = key;
		}

		public UnavailableSessionException(StoreSessionCacheKey key, Exception innerException) : base(Strings.UnavailableSession(key), innerException)
		{
			this.key = key;
		}

		protected UnavailableSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
