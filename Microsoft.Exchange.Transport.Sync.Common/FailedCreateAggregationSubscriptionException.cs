using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedCreateAggregationSubscriptionException : LocalizedException
	{
		public FailedCreateAggregationSubscriptionException(string name) : base(Strings.FailedCreateAggregationSubscription(name))
		{
			this.name = name;
		}

		public FailedCreateAggregationSubscriptionException(string name, Exception innerException) : base(Strings.FailedCreateAggregationSubscription(name), innerException)
		{
			this.name = name;
		}

		protected FailedCreateAggregationSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
