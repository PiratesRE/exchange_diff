using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MultipleNativeItemsHaveSameCloudIdException : LocalizedException
	{
		public MultipleNativeItemsHaveSameCloudIdException(string cloudId, Guid subscriptionGuid) : base(Strings.MultipleNativeItemsHaveSameCloudIdException(cloudId, subscriptionGuid))
		{
			this.cloudId = cloudId;
			this.subscriptionGuid = subscriptionGuid;
		}

		public MultipleNativeItemsHaveSameCloudIdException(string cloudId, Guid subscriptionGuid, Exception innerException) : base(Strings.MultipleNativeItemsHaveSameCloudIdException(cloudId, subscriptionGuid), innerException)
		{
			this.cloudId = cloudId;
			this.subscriptionGuid = subscriptionGuid;
		}

		protected MultipleNativeItemsHaveSameCloudIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cloudId = (string)info.GetValue("cloudId", typeof(string));
			this.subscriptionGuid = (Guid)info.GetValue("subscriptionGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cloudId", this.cloudId);
			info.AddValue("subscriptionGuid", this.subscriptionGuid);
		}

		public string CloudId
		{
			get
			{
				return this.cloudId;
			}
		}

		public Guid SubscriptionGuid
		{
			get
			{
				return this.subscriptionGuid;
			}
		}

		private readonly string cloudId;

		private readonly Guid subscriptionGuid;
	}
}
