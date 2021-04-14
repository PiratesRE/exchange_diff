using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotAttachSelfException : ServicePermanentException
	{
		public CannotAttachSelfException() : base(CoreResources.IDs.ErrorCannotAttachSelf)
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
