using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class PropertyRequestFailedException : ServicePermanentExceptionWithPropertyPath
	{
		public PropertyRequestFailedException(Enum messageId) : base(ResponseCodeType.ErrorItemPropertyRequestFailed, messageId)
		{
		}

		public PropertyRequestFailedException(Enum messageId, PropertyPath propertyPath, Exception innerException) : base(ResponseCodeType.ErrorItemPropertyRequestFailed, messageId, propertyPath, innerException)
		{
		}

		public PropertyRequestFailedException(Enum messageId, PropertyPath propertyPath) : base(PropertyRequestFailedException.GetResponseCodeType(messageId), messageId, propertyPath)
		{
		}

		private static ResponseCodeType GetResponseCodeType(Enum messageId)
		{
			ResponseCodeType result = ResponseCodeType.ErrorItemPropertyRequestFailed;
			CoreResources.IDs ds = (CoreResources.IDs)messageId;
			CoreResources.IDs ds2 = ds;
			if (ds2 != CoreResources.IDs.ErrorItemPropertyRequestFailed)
			{
				if (ds2 == (CoreResources.IDs)2370747299U)
				{
					result = ResponseCodeType.ErrorFolderPropertRequestFailed;
				}
			}
			else
			{
				result = ResponseCodeType.ErrorItemPropertyRequestFailed;
			}
			return result;
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
