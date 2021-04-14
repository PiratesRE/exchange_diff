using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SubscriptionCacheOperationFailedException : LocalizedException
	{
		public SubscriptionCacheOperationFailedException(LocalizedString info) : base(Strings.SubscriptionCacheOperationFailed(info))
		{
			this.info = info;
		}

		public SubscriptionCacheOperationFailedException(LocalizedString info, Exception innerException) : base(Strings.SubscriptionCacheOperationFailed(info), innerException)
		{
			this.info = info;
		}

		protected SubscriptionCacheOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.info = (LocalizedString)info.GetValue("info", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("info", this.info);
		}

		public LocalizedString Info
		{
			get
			{
				return this.info;
			}
		}

		private readonly LocalizedString info;
	}
}
