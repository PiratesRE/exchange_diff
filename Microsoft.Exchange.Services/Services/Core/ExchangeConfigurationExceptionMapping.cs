using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExchangeConfigurationExceptionMapping : ExceptionMappingBase
	{
		public ExchangeConfigurationExceptionMapping(Type exceptionType, ExchangeVersion effectiveVersion, ResponseCodeType responseCode) : base(exceptionType, ExceptionMappingBase.Attributes.None)
		{
			this.effectiveVersion = effectiveVersion;
			this.responseCode = responseCode;
		}

		public override LocalizedString GetLocalizedMessage(LocalizedException exception)
		{
			return exception.LocalizedString;
		}

		protected override ResponseCodeType GetResponseCode(LocalizedException exception)
		{
			return this.responseCode;
		}

		protected override ExchangeVersion GetEffectiveVersion(LocalizedException exception)
		{
			return this.effectiveVersion;
		}

		protected override CoreResources.IDs GetResourceId(LocalizedException exception)
		{
			return CoreResources.IDs.ErrorExchangeConfigurationException;
		}

		private readonly ExchangeVersion effectiveVersion;

		private readonly ResponseCodeType responseCode;
	}
}
