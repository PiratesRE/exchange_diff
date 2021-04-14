using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class ReminderNotificationPayload : NotificationPayloadBase
	{
		public ReminderNotificationPayload()
		{
		}

		public ReminderNotificationPayload(bool shouldGetReminders)
		{
			this.shouldGetReminders = shouldGetReminders;
		}

		public bool ShouldGetReminders
		{
			get
			{
				return this.shouldGetReminders;
			}
			set
			{
				this.shouldGetReminders = value;
			}
		}

		public bool Reload
		{
			get
			{
				return this.reload;
			}
			set
			{
				this.reload = value;
			}
		}

		[DataMember]
		private bool shouldGetReminders;

		[DataMember(EmitDefaultValue = false)]
		private bool reload;
	}
}
