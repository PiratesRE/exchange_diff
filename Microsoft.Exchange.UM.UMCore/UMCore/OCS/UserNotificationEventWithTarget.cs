using System;
using System.Xml;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal abstract class UserNotificationEventWithTarget : UserNotificationEvent
	{
		internal UserNotificationEventWithTarget(string user, Guid recipientObjectGuid, Guid tenantGuid, XmlNode eventData) : base(user, recipientObjectGuid, tenantGuid, eventData)
		{
			this.target = base.FindTarget(UserNotificationEvent.GetNodeValue(eventData, "Target"));
		}

		protected string Target
		{
			get
			{
				return this.target;
			}
		}

		protected override string MessageClass
		{
			get
			{
				return "IPM.Note.Microsoft.Conversation.Voice";
			}
		}

		private string target;
	}
}
