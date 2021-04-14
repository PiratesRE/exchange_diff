using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal class TeamPickUpUserNotificationEvent : UserNotificationEventWithTarget
	{
		internal TeamPickUpUserNotificationEvent(string user, Guid recipientObjectGuid, Guid tenantGuid, string template, XmlNode eventData) : base(user, recipientObjectGuid, tenantGuid, eventData)
		{
		}

		protected override void InternalRenderMessage(MessageContentBuilder content, MessageItem message, ContactInfo callerInfo)
		{
			message.Subject = content.GetTeamPickUpSubject(callerInfo, base.CallerId, base.Target, base.Subject);
			content.AddTeamPickUpBody(base.Target, base.CallerId, callerInfo);
		}
	}
}
