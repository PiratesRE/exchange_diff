using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetEmailSignature : ServiceCommand<EmailSignatureConfiguration>
	{
		public GetEmailSignature(CallContext callContext) : base(callContext)
		{
		}

		protected override EmailSignatureConfiguration InternalExecute()
		{
			return GetEmailSignature.GetSetting(base.CallContext);
		}

		public static EmailSignatureConfiguration GetSetting(CallContext callContext)
		{
			EmailSignatureConfiguration emailSignatureConfiguration = new EmailSignatureConfiguration();
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.AutoAddSignatureOnMobile);
			UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.SignatureTextOnMobile);
			UserConfigurationPropertyDefinition propertyDefinition3 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.UseDesktopSignature);
			UserConfigurationPropertyDefinition propertyDefinition4 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.SignatureText);
			UserOptionsType userOptionsType = new UserOptionsType();
			userOptionsType.Load(callContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition,
				propertyDefinition2,
				propertyDefinition3,
				propertyDefinition4
			});
			emailSignatureConfiguration.AutoAddSignature = userOptionsType.AutoAddSignatureOnMobile;
			emailSignatureConfiguration.SignatureText = userOptionsType.SignatureTextOnMobile;
			emailSignatureConfiguration.UseDesktopSignature = userOptionsType.UseDesktopSignature;
			emailSignatureConfiguration.DesktopSignatureText = userOptionsType.SignatureText;
			return emailSignatureConfiguration;
		}
	}
}
