using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class AQSParserExceptionMapping : ExceptionMappingBase
	{
		public AQSParserExceptionMapping() : base(typeof(ParserException), ExceptionMappingBase.Attributes.StopsBatchProcessing)
		{
		}

		protected override ResponseCodeType GetResponseCode(LocalizedException exception)
		{
			return ResponseCodeType.ErrorInvalidArgument;
		}

		protected override ExchangeVersion GetEffectiveVersion(LocalizedException exception)
		{
			return ExchangeVersion.Exchange2010;
		}

		protected override CoreResources.IDs GetResourceId(LocalizedException exception)
		{
			return (CoreResources.IDs)0U;
		}

		public override LocalizedString GetLocalizedMessage(LocalizedException exception)
		{
			return exception.LocalizedString;
		}
	}
}
