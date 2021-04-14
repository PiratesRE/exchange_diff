using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsRawNotification : WnsNotification
	{
		public WnsRawNotification(string appId, OrganizationId tenantId, string deviceUri, string data) : base(appId, tenantId, deviceUri)
		{
			this.Data = data;
		}

		public string Data { get; private set; }

		protected override string GetSerializedPayload(List<LocalizedString> errors)
		{
			return this.Data;
		}

		protected override void PrepareWnsRequest(WnsRequest wnsRequest)
		{
			wnsRequest.WnsType = "wns/raw";
			wnsRequest.ContentType = "application/octet-stream";
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; data:{1};", base.InternalToFullString(), this.Data.ToNullableString());
		}
	}
}
