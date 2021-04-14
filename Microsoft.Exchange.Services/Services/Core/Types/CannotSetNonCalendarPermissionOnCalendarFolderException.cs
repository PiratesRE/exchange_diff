using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotSetNonCalendarPermissionOnCalendarFolderException : ServicePermanentException
	{
		public CannotSetNonCalendarPermissionOnCalendarFolderException() : base(CoreResources.IDs.ErrorCannotSetNonCalendarPermissionOnCalendarFolder)
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
