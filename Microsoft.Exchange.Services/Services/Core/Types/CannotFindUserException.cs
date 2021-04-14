using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotFindUserException : ServicePermanentException
	{
		public CannotFindUserException(Enum messageId) : base(ResponseCodeType.ErrorCannotFindUser, messageId)
		{
		}

		public CannotFindUserException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotFindUser, messageId, innerException)
		{
		}

		public CannotFindUserException(ResponseCodeType responseCodeType, LocalizedString message) : base(responseCodeType, message)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}
