using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidValueForPropertyException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidValueForPropertyException(Enum messageId) : base(ResponseCodeType.ErrorInvalidValueForProperty, messageId)
		{
		}

		public InvalidValueForPropertyException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidValueForProperty, messageId, innerException)
		{
		}

		public InvalidValueForPropertyException(PropertyPath propertyPath, Exception innerException) : base(CoreResources.IDs.ErrorInvalidValueForProperty, propertyPath, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		internal static InvalidValueForPropertyException CreateDuplicateDictionaryKeyError(Enum messageId, string dictionaryKey, Exception innerException)
		{
			return new InvalidValueForPropertyException(messageId, innerException)
			{
				ConstantValues = 
				{
					{
						"DictionaryKey",
						dictionaryKey
					}
				}
			};
		}

		internal static InvalidValueForPropertyException CreateConversionError(Enum messageId, string valueToConvert, string convertToType, Exception innerException)
		{
			return new InvalidValueForPropertyException(messageId, innerException)
			{
				ConstantValues = 
				{
					{
						"ValueToConvert",
						valueToConvert
					},
					{
						"ConvertToType",
						convertToType
					}
				}
			};
		}
	}
}
