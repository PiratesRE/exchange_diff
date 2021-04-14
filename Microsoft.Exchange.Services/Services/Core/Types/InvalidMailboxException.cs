using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidMailboxException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidMailboxException(int memberIndex, PropertyPath propertyPath, LocalizedException innerException, Enum messageId) : base(ResponseCodeType.ErrorInvalidMailbox, messageId, propertyPath, innerException)
		{
			base.ConstantValues.Add("MemberIndex", memberIndex.ToString());
			ServiceError serviceError = ServiceErrors.GetServiceError(innerException);
			base.ConstantValues.Add("Inner.ResponseCode", serviceError.MessageKey.ToString());
			base.ConstantValues.Add("Inner.MessageText", serviceError.MessageText);
			base.ConstantValues.Add("Inner.DescriptiveLinkKey", serviceError.DescriptiveLinkId.ToString());
		}

		public InvalidMailboxException(PropertyPath propertyPath, string property, string actualValue) : base(CoreResources.IDs.ErrorInvalidMailbox, propertyPath)
		{
			base.ConstantValues.Add("Property", property);
			base.ConstantValues.Add("Value", actualValue);
		}

		public InvalidMailboxException(PropertyPath propertyPath, string property, string expectedValue, string actualValue, Enum messageId) : base(ResponseCodeType.ErrorInvalidMailbox, messageId, propertyPath)
		{
			base.ConstantValues.Add("Property", property);
			base.ConstantValues.Add("Value", actualValue);
			base.ConstantValues.Add("ExpectedValue", expectedValue);
		}

		public InvalidMailboxException(PropertyPath propertyPath, Enum messageId) : base(ResponseCodeType.ErrorInvalidMailbox, messageId, propertyPath)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
