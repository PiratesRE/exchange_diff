using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class WnsXmlNotification : WnsNotification
	{
		public WnsXmlNotification(string appId, OrganizationId tenantId, string deviceUri) : base(appId, tenantId, deviceUri)
		{
		}

		protected abstract void WriteWnsXmlPayload(WnsPayloadWriter wpw);

		protected override void PrepareWnsRequest(WnsRequest wnsRequest)
		{
			wnsRequest.ContentType = "text/xml";
		}

		protected sealed override string GetSerializedPayload(List<LocalizedString> errors)
		{
			WnsPayloadWriter wnsPayloadWriter = new WnsPayloadWriter();
			this.WriteWnsXmlPayload(wnsPayloadWriter);
			if (wnsPayloadWriter.IsValid)
			{
				return wnsPayloadWriter.Dump();
			}
			errors.AddRange(wnsPayloadWriter.ValidationErrors);
			return null;
		}
	}
}
