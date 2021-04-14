using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class StaticExceptionMapping : ExceptionMappingBase
	{
		public StaticExceptionMapping(Type exceptionType, ExceptionMappingBase.Attributes attributes, ExchangeVersion effectiveVersion, ResponseCodeType responseCode, CoreResources.IDs messageId) : base(exceptionType, attributes)
		{
			this.effectiveVersion = effectiveVersion;
			this.responseCode = responseCode;
			this.messageId = messageId;
		}

		public StaticExceptionMapping(Type exceptionType, ExchangeVersion effectiveVersion, ResponseCodeType responseCode, CoreResources.IDs messageId) : this(exceptionType, ExceptionMappingBase.Attributes.None, effectiveVersion, responseCode, messageId)
		{
		}

		public StaticExceptionMapping(Type exceptionType, ExceptionMappingBase.Attributes attributes, ResponseCodeType responseCode, CoreResources.IDs messageId) : this(exceptionType, attributes, ExchangeVersion.Exchange2007, responseCode, messageId)
		{
		}

		public StaticExceptionMapping(Type exceptionType, ResponseCodeType responseCode, CoreResources.IDs messageId) : this(exceptionType, ExceptionMappingBase.Attributes.None, ExchangeVersion.Exchange2007, responseCode, messageId)
		{
		}

		public ResponseCodeType ResponseCode
		{
			get
			{
				return this.responseCode;
			}
		}

		public LocalizedString MessageText
		{
			get
			{
				return this.GetLocalizedMessage(this.messageId);
			}
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
			return this.messageId;
		}

		private ExchangeVersion effectiveVersion;

		private ResponseCodeType responseCode;

		private CoreResources.IDs messageId;
	}
}
