using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetLayoutSettings : ServiceCommand<bool>
	{
		public SetLayoutSettings(CallContext callContext, LayoutSettingsType newSettings) : base(callContext)
		{
			this.newSettings = newSettings;
		}

		protected override bool InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			new ConfigurationContext(userContext);
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ShowSenderOnTopInListView);
			UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ShowPreviewTextInListView);
			UserConfigurationPropertyDefinition propertyDefinition3 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ConversationSortOrder);
			UserConfigurationPropertyDefinition propertyDefinition4 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.HideDeletedItems);
			UserConfigurationPropertyDefinition propertyDefinition5 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.GlobalReadingPanePosition);
			UserConfigurationPropertyDefinition propertyDefinition6 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ShowReadingPaneOnFirstLoad);
			new UserOptionsType
			{
				ShowSenderOnTopInListView = this.newSettings.ShowSenderOnTopInListView,
				ShowPreviewTextInListView = this.newSettings.ShowPreviewTextInListView,
				ConversationSortOrder = this.newSettings.ConversationSortOrder,
				HideDeletedItems = this.newSettings.HideDeletedItems,
				GlobalReadingPanePosition = this.newSettings.GlobalReadingPanePosition,
				ShowReadingPaneOnFirstLoad = this.newSettings.ShowFirstMessageOnSignIn
			}.Commit(mailboxIdentityMailboxSession, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition,
				propertyDefinition2,
				propertyDefinition3,
				propertyDefinition4,
				propertyDefinition5,
				propertyDefinition6
			});
			InferenceSettingsType.UpdateUserPreferenceFlag(mailboxIdentityMailboxSession, userContext, this.newSettings.ShowInferenceUiElements);
			return true;
		}

		private LayoutSettingsType newSettings;
	}
}
