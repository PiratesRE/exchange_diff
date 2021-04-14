using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MultiLegacyMailboxAccessException : ServicePermanentException
	{
		public MultiLegacyMailboxAccessException() : base(CoreResources.IDs.ErrorMultiLegacyMailboxAccess)
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
