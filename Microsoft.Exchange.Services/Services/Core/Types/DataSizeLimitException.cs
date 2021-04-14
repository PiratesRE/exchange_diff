using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class DataSizeLimitException : ServicePermanentExceptionWithPropertyPath
	{
		public DataSizeLimitException(PropertyPath propertyPath) : base((CoreResources.IDs)2935460503U, propertyPath)
		{
		}

		public DataSizeLimitException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorDataSizeLimitExceeded, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		internal const int DataSizeLimit = 2147483647;
	}
}
