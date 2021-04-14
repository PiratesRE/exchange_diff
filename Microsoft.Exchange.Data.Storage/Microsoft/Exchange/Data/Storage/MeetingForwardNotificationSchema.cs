using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingForwardNotificationSchema : MeetingMessageInstanceSchema
	{
		public new static MeetingForwardNotificationSchema Instance
		{
			get
			{
				if (MeetingForwardNotificationSchema.instance == null)
				{
					MeetingForwardNotificationSchema.instance = new MeetingForwardNotificationSchema();
				}
				return MeetingForwardNotificationSchema.instance;
			}
		}

		protected MeetingForwardNotificationSchema()
		{
			base.RemoveConstraints(CalendarItemSchema.Instance.Constraints);
		}

		[Autoload]
		internal static readonly StorePropertyDefinition ForwardNotificationRecipients = InternalSchema.ForwardNotificationRecipients;

		[Autoload]
		internal static readonly StorePropertyDefinition MFNAddedRecipients = InternalSchema.MFNAddedRecipients;

		private static MeetingForwardNotificationSchema instance = null;
	}
}
