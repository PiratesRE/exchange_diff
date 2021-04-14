using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PendingGetNotification : PushNotification
	{
		public PendingGetNotification(string appId, OrganizationId tenantId, string subscriptionId, PendingGetPayload payload) : base(appId, tenantId)
		{
			this.SubscriptionId = subscriptionId;
			this.Payload = payload;
		}

		public string SubscriptionId { get; private set; }

		public PendingGetPayload Payload { get; private set; }

		public override string RecipientId
		{
			get
			{
				return this.SubscriptionId;
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; subscriptionId:{1}; payload:{2}", base.InternalToFullString(), this.SubscriptionId.ToNullableString(), this.Payload.ToNullableString(null));
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (string.IsNullOrEmpty(this.SubscriptionId))
			{
				errors.Add(Strings.InvalidSubscriptionId);
			}
			if (this.Payload == null)
			{
				errors.Add(Strings.InvalidPayload);
			}
			if (this.Payload.EmailCount == null)
			{
				errors.Add(Strings.InvalidEmailCount);
			}
		}
	}
}
