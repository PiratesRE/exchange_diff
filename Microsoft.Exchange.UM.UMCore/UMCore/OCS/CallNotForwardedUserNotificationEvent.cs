using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal class CallNotForwardedUserNotificationEvent : UserNotificationEventWithTarget
	{
		internal CallNotForwardedUserNotificationEvent(string user, Guid recipientObjectGuid, Guid tenantGuid, string template, XmlNode eventData) : base(user, recipientObjectGuid, tenantGuid, eventData)
		{
		}

		protected override string MessageClass
		{
			get
			{
				return string.Empty;
			}
		}

		protected override void InternalRenderMessage(MessageContentBuilder content, MessageItem message, ContactInfo callerInfo)
		{
			message.Subject = content.GetCallNotForwardedSubject(callerInfo, base.CallerId, base.Target, base.Subject);
			content.AddCallNotForwardedBody(base.Target, base.CallerId, callerInfo);
		}
	}
}
