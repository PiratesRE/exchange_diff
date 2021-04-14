using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ObjectCorruptException : ServicePermanentException
	{
		public ObjectCorruptException(Exception innerException, bool useItemError) : base(useItemError ? ((CoreResources.IDs)2624402344U) : ((CoreResources.IDs)2966054199U), innerException)
		{
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
