using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnifiedGroupAlreadyExistsException : LocalizedException
	{
		public UnifiedGroupAlreadyExistsException() : base(CoreResources.GetLocalizedString((CoreResources.IDs)2930851601U))
		{
		}

		public UnifiedGroupAlreadyExistsException(Exception innerException) : base(CoreResources.GetLocalizedString((CoreResources.IDs)2930851601U), innerException)
		{
		}
	}
}
