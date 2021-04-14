using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetEmailSignature : ServiceCommand<bool>
	{
		public SetEmailSignature(CallContext callContext, EmailSignatureConfiguration userEmailSignature) : base(callContext)
		{
			this.newAutoAddSignature = userEmailSignature.AutoAddSignature;
			this.newSignatureText = userEmailSignature.SignatureText;
			this.useDesktopSignature = userEmailSignature.UseDesktopSignature;
		}

		protected override bool InternalExecute()
		{
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.AutoAddSignatureOnMobile);
			UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.SignatureTextOnMobile);
			UserConfigurationPropertyDefinition propertyDefinition3 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.UseDesktopSignature);
			new UserOptionsType
			{
				AutoAddSignatureOnMobile = this.newAutoAddSignature,
				SignatureTextOnMobile = this.newSignatureText,
				UseDesktopSignature = this.useDesktopSignature
			}.Commit(base.CallContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition,
				propertyDefinition2,
				propertyDefinition3
			});
			return true;
		}

		private readonly bool newAutoAddSignature;

		private readonly string newSignatureText;

		private readonly bool useDesktopSignature;
	}
}
