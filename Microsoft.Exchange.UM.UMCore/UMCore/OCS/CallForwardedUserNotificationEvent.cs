using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal class CallForwardedUserNotificationEvent : TeamPickUpUserNotificationEvent
	{
		internal CallForwardedUserNotificationEvent(string user, Guid recipientObjectGuid, Guid tenantGuid, string template, XmlNode eventData) : base(user, recipientObjectGuid, tenantGuid, template, eventData)
		{
		}

		protected override void InternalRenderMessage(MessageContentBuilder content, MessageItem message, ContactInfo callerInfo)
		{
			message.Subject = content.GetCallForwardedSubject(callerInfo, base.CallerId, base.Target, base.Subject);
			content.AddCallForwardedBody(base.Target, base.CallerId, callerInfo);
		}
	}
}
