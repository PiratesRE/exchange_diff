using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidManagementRoleHeaderException : ServicePermanentException
	{
		public InvalidManagementRoleHeaderException() : base((CoreResources.IDs)2674011741U)
		{
		}

		public InvalidManagementRoleHeaderException(Enum messageId) : base(ResponseCodeType.ErrorInvalidManagementRoleHeader, messageId)
		{
		}

		public InvalidManagementRoleHeaderException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidManagementRoleHeader, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
