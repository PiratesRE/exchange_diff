using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetNotificationSettings : ServiceCommand<bool>
	{
		public SetNotificationSettings(CallContext callContext, NotificationSettingsRequest settings) : base(callContext)
		{
			this.settings = settings;
		}

		protected override bool InternalExecute()
		{
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.EnableReminders);
			UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.EnableReminderSound);
			UserConfigurationPropertyDefinition propertyDefinition3 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.NewItemNotify);
			new UserOptionsType
			{
				EnableReminders = this.settings.EnableReminders,
				EnableReminderSound = this.settings.EnableReminderSound,
				NewItemNotify = (NewNotification)this.settings.NewItemNotify
			}.Commit(base.CallContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition,
				propertyDefinition2,
				propertyDefinition3
			});
			return true;
		}

		private NotificationSettingsRequest settings;
	}
}
