using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotSetCalendarPermissionOnNonCalendarFolderException : ServicePermanentException
	{
		public CannotSetCalendarPermissionOnNonCalendarFolderException() : base((CoreResources.IDs)2183377470U)
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
