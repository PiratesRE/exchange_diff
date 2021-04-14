using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WebAppNotification : PushNotification
	{
		public WebAppNotification(string appId, OrganizationId tenantId, string action, string payload) : base(appId, tenantId)
		{
			this.Action = action;
			this.Payload = payload;
		}

		public string Action { get; private set; }

		public string Payload { get; private set; }

		public override string RecipientId
		{
			get
			{
				return this.Action;
			}
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (this.Payload == null)
			{
				errors.Add(Strings.InvalidPayload);
				return;
			}
			if (string.IsNullOrWhiteSpace(this.Action))
			{
				errors.Add(Strings.WebAppInvalidAction);
			}
			if (string.IsNullOrWhiteSpace(this.Payload))
			{
				errors.Add(Strings.WebAppInvalidPayload);
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; payload:{1}", base.InternalToFullString(), this.Payload.ToNullableString());
		}
	}
}
