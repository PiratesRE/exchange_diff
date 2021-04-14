using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class ServicePermanentException : LocalizedException
	{
		public ServicePermanentException(ResponseCodeType responseCode, LocalizedString message) : base(message)
		{
			this.responseCode = responseCode;
		}

		public ServicePermanentException(ResponseCodeType responseCode, Enum messageId) : base(CoreResources.GetLocalizedString((CoreResources.IDs)messageId))
		{
			this.responseCode = responseCode;
		}

		public ServicePermanentException(ResponseCodeType responseCode, Enum messageId, Exception innerException) : base(CoreResources.GetLocalizedString((CoreResources.IDs)messageId), innerException)
		{
			this.responseCode = responseCode;
		}

		public ServicePermanentException(Enum messageId) : base(CoreResources.GetLocalizedString((CoreResources.IDs)messageId))
		{
			this.responseCode = (ResponseCodeType)Enum.Parse(typeof(ResponseCodeType), messageId.ToString(), true);
		}

		public ServicePermanentException(Enum messageId, Exception innerException) : base(CoreResources.GetLocalizedString((CoreResources.IDs)messageId), innerException)
		{
			this.responseCode = (ResponseCodeType)Enum.Parse(typeof(ResponseCodeType), messageId.ToString(), true);
		}

		public ServicePermanentException(Enum messageId, LocalizedString message, Exception innerException) : base(message, innerException)
		{
			this.responseCode = (ResponseCodeType)Enum.Parse(typeof(ResponseCodeType), messageId.ToString(), true);
		}

		public ResponseCodeType ResponseCode
		{
			get
			{
				return this.responseCode;
			}
		}

		public IDictionary<string, string> ConstantValues
		{
			get
			{
				return this.constantValues;
			}
		}

		internal abstract ExchangeVersion EffectiveVersion { get; }

		internal virtual bool StopsBatchProcessing
		{
			get
			{
				return false;
			}
		}

		internal virtual ServiceError CreateServiceError(ExchangeVersion requestVersion)
		{
			IProvidePropertyPaths providePropertyPaths = this as IProvidePropertyPaths;
			IProvideXmlNodeArray provideXmlNodeArray = this as IProvideXmlNodeArray;
			return new ServiceError(base.LocalizedString, this.ResponseCode, 0, requestVersion, this.EffectiveVersion, this.StopsBatchProcessing, (providePropertyPaths != null) ? providePropertyPaths.PropertyPaths : null, this.ConstantValues, (provideXmlNodeArray != null) ? provideXmlNodeArray.NodeArray : null);
		}

		private readonly ResponseCodeType responseCode;

		private Dictionary<string, string> constantValues = new Dictionary<string, string>();
	}
}
