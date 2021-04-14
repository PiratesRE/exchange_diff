using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedPathForSortGroupException : ServicePermanentExceptionWithPropertyPath
	{
		public UnsupportedPathForSortGroupException(PropertyPath offendingPath) : base(CoreResources.IDs.ErrorUnsupportedPathForSortGroup, offendingPath)
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
