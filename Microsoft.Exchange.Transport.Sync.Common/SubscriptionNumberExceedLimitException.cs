using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionNumberExceedLimitException : LocalizedException
	{
		public SubscriptionNumberExceedLimitException(int number) : base(Strings.SubscriptionNumberExceedLimit(number))
		{
			this.number = number;
		}

		public SubscriptionNumberExceedLimitException(int number, Exception innerException) : base(Strings.SubscriptionNumberExceedLimit(number), innerException)
		{
			this.number = number;
		}

		protected SubscriptionNumberExceedLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.number = (int)info.GetValue("number", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("number", this.number);
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		private readonly int number;
	}
}
