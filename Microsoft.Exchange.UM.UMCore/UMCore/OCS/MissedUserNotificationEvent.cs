using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal class MissedUserNotificationEvent : UserNotificationEvent
	{
		internal MissedUserNotificationEvent(string user, Guid recipientObjectGuid, Guid tenantGuid, XmlNode eventData) : base(user, recipientObjectGuid, tenantGuid, eventData)
		{
		}

		protected override string MessageClass
		{
			get
			{
				return "IPM.Note.Microsoft.Missed.Voice";
			}
		}

		protected override void InternalRenderMessage(MessageContentBuilder content, MessageItem message, ContactInfo callerInfo)
		{
			message.Subject = content.GetMissedCallSubject(base.Subject);
			content.AddMissedCallBody(base.CallerId, callerInfo);
		}
	}
}
