using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ResolveNamesExceptionInvalidFolderType : ServicePermanentException
	{
		public ResolveNamesExceptionInvalidFolderType() : base(CoreResources.IDs.ErrorResolveNamesInvalidFolderType)
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
