using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ResolveNamesExceptionOnlyOneContactsFolderAllowed : ServicePermanentException
	{
		public ResolveNamesExceptionOnlyOneContactsFolderAllowed() : base((CoreResources.IDs)2683464521U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
