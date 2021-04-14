using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetOptionSummary : ServiceCommand<OptionSummary>
	{
		public GetOptionSummary(CallContext callContext) : base(callContext)
		{
		}

		protected override OptionSummary InternalExecute()
		{
			OptionSummary optionSummary = new OptionSummary();
			string currentTimeZone = GetTimeZone.GetSetting(base.CallContext, false).CurrentTimeZone;
			ExTimeZone exTimeZone;
			if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(currentTimeZone, out exTimeZone))
			{
				optionSummary.TimeZone = exTimeZone.LocalizableDisplayName.ToString(CultureInfo.CurrentUICulture);
			}
			else
			{
				optionSummary.TimeZone = currentTimeZone;
			}
			UserOofSettingsType setting = GetUserOofSettings.GetSetting(base.MailboxIdentityMailboxSession, exTimeZone);
			optionSummary.Oof = setting.IsOofOn;
			EmailSignatureConfiguration setting2 = GetEmailSignature.GetSetting(base.CallContext);
			optionSummary.Signature = new EmailSignatureConfiguration();
			optionSummary.Signature.AutoAddSignature = setting2.AutoAddSignature;
			optionSummary.Signature.UseDesktopSignature = setting2.UseDesktopSignature;
			optionSummary.Signature.SignatureText = setting2.SignatureText;
			optionSummary.Signature.DesktopSignatureText = setting2.DesktopSignatureText;
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.AlwaysShowBcc);
			UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.AlwaysShowFrom);
			UserConfigurationPropertyDefinition propertyDefinition3 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ShowSenderOnTopInListView);
			UserConfigurationPropertyDefinition propertyDefinition4 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ShowPreviewTextInListView);
			UserOptionsType userOptionsType = new UserOptionsType();
			userOptionsType.Load(base.CallContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition,
				propertyDefinition2,
				propertyDefinition3,
				propertyDefinition4
			});
			optionSummary.AlwaysShowBcc = userOptionsType.AlwaysShowBcc;
			optionSummary.AlwaysShowFrom = userOptionsType.AlwaysShowFrom;
			optionSummary.ShowSenderOnTopInListView = userOptionsType.ShowSenderOnTopInListView;
			optionSummary.ShowPreviewTextInListView = userOptionsType.ShowPreviewTextInListView;
			return optionSummary;
		}
	}
}
