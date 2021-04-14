using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaExceptionMapper : ExceptionMappingBase
	{
		public OwaExceptionMapper(Type exceptionType) : base(exceptionType, ExceptionMappingBase.Attributes.None)
		{
		}

		public override LocalizedString GetLocalizedMessage(LocalizedException exception)
		{
			return exception.LocalizedString;
		}

		protected override ResponseCodeType GetResponseCode(LocalizedException exception)
		{
			return ResponseCodeType.ErrorInternalServerError;
		}

		protected override ExchangeVersion GetEffectiveVersion(LocalizedException exception)
		{
			return ExchangeVersion.Exchange2012;
		}

		protected override CoreResources.IDs GetResourceId(LocalizedException exception)
		{
			return (CoreResources.IDs)0U;
		}
	}
}
