using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedSetAggregationSubscriptionException : LocalizedException
	{
		public FailedSetAggregationSubscriptionException(string name) : base(Strings.FailedSetAggregationSubscription(name))
		{
			this.name = name;
		}

		public FailedSetAggregationSubscriptionException(string name, Exception innerException) : base(Strings.FailedSetAggregationSubscription(name), innerException)
		{
			this.name = name;
		}

		protected FailedSetAggregationSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
