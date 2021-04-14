using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class WrongServerVersionDelegateException : ServicePermanentException
	{
		public WrongServerVersionDelegateException() : base(ResponseCodeType.ErrorWrongServerVersionDelegate, (CoreResources.IDs)3778961523U)
		{
		}

		public WrongServerVersionDelegateException(Exception innerException) : base(ResponseCodeType.ErrorWrongServerVersionDelegate, (CoreResources.IDs)3778961523U, innerException)
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
